/*
*  Copyright (c) 2015 The WebRTC project authors. All Rights Reserved.
*
*  Use of this source code is governed by a BSD-style license
*  that can be found in the LICENSE file in the root of the source
*  tree.
*/

// This code is adapted from
// https://rawgit.com/Miguelao/demos/master/mediarecorder.html

'use strict';

/* globals MediaRecorder */

let mediaRecorder;
let recordedBlobs;

const codecPreferences = document.querySelector('#codecPreferences');
const errorMsgElement = document.querySelector('span#errorMsg');
//const recordedVideo = document.querySelector('video#recorded');
const recordedVideo = document.querySelector('video#theVideo');

const recordButton = document.querySelector('button#record');

// Start Recording

recordButton.addEventListener('click', () => {
    if (recordButton.textContent === 'Start Recording')
    {
        // Added for the iphone
        stopCamera();
        // const mimeType = codecPreferences.options[codecPreferences.selectedIndex].value.split(';', 1)[0];
        // const superBuffer = new Blob(recordedBlobs, { type: mimeType });
        recordedVideo.src = null;
        recordedVideo.srcObject = null;
        // End of iphone

        startRecording();
    }
    else
    {
        stopRecording();
    }
});

const playButton = document.querySelector('button#play');

function ClickPlay() {
    playButton.click();
}

playButton.addEventListener('click', () =>
{
    // If they want to play while still recording, stop the recording:
    if (recordButton.textContent === 'Stop Recording') {
        stopRecording();
        setTimeout(ClickPlay, 1000);
    }

    stopCamera();
    //    const superBuffer = new Blob(recordedBlobs, { type: mimeType });
    recordedVideo.src = null;
    recordedVideo.srcObject = null;

    recordedVideo.src = URL.createObjectURL(new Blob(recordedBlobs, { type: mediaRecorder.mimeType }));
    // recordedVideo.src = window.URL.createObjectURL(superBuffer);
    recordedVideo.controls = true;
    recordedVideo.muted = false;
    recordedVideo.play();
});


function downloadBlob1() {
    const blob = new Blob(recordedBlobs, { type: 'video/webm' });
    // Reserved for downloading an object
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.style.display = 'none';
    a.href = url;
    a.download = 'test.webm';
    document.body.appendChild(a);
    a.click();

    setTimeout(() => {
            document.body.removeChild(a);
            window.URL.revokeObjectURL(url);
        }, 100);
}

const uploadButton = document.querySelector('button#btnUpload');
uploadButton.addEventListener('click', () => {
    const blob = new Blob(recordedBlobs, { type: 'video/webm' });
    PostBlob(blob);
});

function handleDataAvailable(event) {
    console.log('handleDataAvailable', event);
    if (event.data && event.data.size > 0) {
        recordedBlobs.push(event.data);
    }
}

function getSupportedMimeTypes() {
    const possibleTypes = [
        'video/webm;codecs=vp9,opus',
        'video/webm;codecs=vp8,opus',
        'video/webm;codecs=h264,opus',
        'video/mp4;codecs=h264,aac',
    ];
    return possibleTypes.filter(mimeType => {
        return MediaRecorder.isTypeSupported(mimeType);
    });
}

var stream;

async function startRecording() {

    await startCamera();
    recordedBlobs = [];
    const oMimeTypes= getSupportedMimeTypes();
    const mimeType = oMimeTypes[0];
    const options = { mimeType };

    try {

        stream = await navigator.mediaDevices.getUserMedia({ audio: true, video: true });
        mediaRecorder = new MediaRecorder(stream);
        recordedVideo.srcObject = stream;

        //mediaRecorder = new MediaRecorder(window.stream, options);
    } catch (e) {
        console.error('Exception while creating MediaRecorder:', e);
        errorMsgElement.innerHTML = `Exception while creating MediaRecorder: ${JSON.stringify(e)}`;
        alert(e);
        return;
    }

    console.log('Created MediaRecorder', mediaRecorder, 'with options', options);
    recordButton.textContent = 'Stop Recording';
    recordedVideo.muted = true;

    mediaRecorder.onstop = (event) => {
        console.log('Recorder stopped: ', event);
        console.log('Recorded Blobs: ', recordedBlobs);
    };
    mediaRecorder.ondataavailable = handleDataAvailable;
    mediaRecorder.start();
    console.log('MediaRecorder started', mediaRecorder);
}

async function stopRecording() {
    await mediaRecorder.stop();
    recordButton.textContent = 'Start Recording';
    uploadButton.disabled = false;
}

function handleSuccess(stream) {
    console.log('getUserMedia() got stream:', stream);
    recordedVideo.srcObject = stream;
    getSupportedMimeTypes().forEach(mimeType => {
        const option = document.createElement('option');
        option.value = mimeType;
        option.innerText = option.value;
        codecPreferences.appendChild(option);
    });
}

function toast(sTitle, sBody) {
    $.toast({ heading: sTitle, text: sBody,
        loader: true, loaderBg: '#9EC600'
    });
}


function PostBlob(blob) {
    // FormData
    var formData = new FormData();  
    formData.append('video-blob', blob);
    var oCategory = document.getElementById("ddCategory");
    var selCat = oCategory.options[oCategory.selectedIndex].value;
    formData.append("ddcategory", selCat);
    // progress-bar
    // POST the Blob  
    $.ajax({
        type: 'POST',
        url: "Web.ashx?action=live",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            if (result) {
                // Redirect the User to videos
                toast('Success', "Your live video has been uploaded successfully!");
                location.href = "VideoList";
            }
        },
        error: function (result) {
            console.log(result);
            // Failure
            alert("The Upload failed " + result);
        }
    })
}

async function init(constraints) {
    try {
        const stream = await navigator.mediaDevices.getUserMedia(constraints);
        handleSuccess(stream);
    } catch (e) {
        console.error('navigator.getUserMedia error:', e);
        errorMsgElement.innerHTML = `navigator.getUserMedia error:${e.toString()}`;
    }
}

async function stopCamera() {
    if (stream) {
        // const mediaStream = video.srcObject;
        await stream.getTracks().forEach(track => track.stop());
        // video.srcObject = null;
    }
}

async function startCamera() {
    const constraints = {
        audio: {
        },
        video: {
            width: 640, height: 480 /*320-640-1280*/
        }
    };
    console.log('Using media constraints:', constraints);
    await init(constraints);
}


