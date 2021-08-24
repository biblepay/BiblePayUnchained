
var xhr;
var exturl;

function getBlob(url) {
    exturl = url;
    return new Promise(function (resolve, reject) {
        xhr = new XMLHttpRequest();
        xhr.open('get', url, true);
        xhr.responseType = "blob";

        xhr.onload = function () {
            var status = xhr.status;
            if (status === 200) {
                console.log("got blob");
                resolve(xhr.response);
            } else {
                console.log("did not get blob");
                reject(status);
            }
        };
        xhr.send();
    });
}

async function myvideograbber(urlsource)
{
    var blob = await getBlob(urlsource);
    console.log("Blob1:" + blob);
    return blob;
}

window.onbeforeunload = function () {
    // Send cancel request
    setRemoteValue('canceldownload', exturl, 'canceldownload', '');

    console.log('aborting2...');
    return null;
};

(async function () {
    // IndexedDB
    var url = "DownloadFile?filename=https://bbp.s3.filebase.com/b0239f12-fa42-4aae-94db-98eb882bd57d.mp4";

    blob = await myvideograbber(url);
    console.log("MyBlob:Got");
    var indexedDB = window.indexedDB || window.webkitIndexedDB || window.mozIndexedDB || window.OIndexedDB || window.msIndexedDB,
        IDBTransaction = window.IDBTransaction || window.webkitIDBTransaction || window.OIDBTransaction || window.msIDBTransaction,
    dbVersion = 1.0;
    // Create/open database
    var request = indexedDB.open("videoFiles", dbVersion);
        var db;
        var createObjectStore = function (dataBase) {
            // Create an objectStore
            console.log("Creating objectStore");
            dataBase.createObjectStore("earth");
        },

        putEarthInDb = function (blob) {
            console.log("Putting earth in IndexedDB");
            var transaction = db.transaction(["earth"], "readwrite");
            var put = transaction.objectStore("earth").put(blob, "video");
            transaction.objectStore("earth").get("video").onsuccess = function (event)
            {
                var vidFile = event.target.result;
                console.log("Got earth!" + vidFile);
                console.log('File Type: ' + vidFile.type); /// THIS SHOWS : application/xml
                var URL = window.URL || window.webkitURL;
                var vidEarth = document.getElementById("video2");
                vidEarth.src = URL.createObjectURL(new Blob([vidFile]));
                //URL.revokeObjectURL(vidURL);
            };
        };

    request.onerror = function (event) {
        console.log("Error creating/accessing IndexedDB database");
    };

    request.onsuccess = function (event) {
        console.log("Success creating/accessing IndexedDB database");
        db = request.result;

        db.onerror = function (event) {
            console.log("Error creating/accessing IndexedDB database");
        };

        // Interim solution for Google Chrome to create an objectStore. Will be deprecated
        if (db.setVersion) {
            if (db.version !== dbVersion) {
                var setVersion = db.setVersion(dbVersion);
                setVersion.onsuccess = function () {
                    createObjectStore(db);
                };
            }
        }
        
        putEarthInDb(blob);


    }

    // For future use. Currently only in latest Firefox versions
    request.onupgradeneeded = function (event) {
        createObjectStore(event.target.result);
    };
})();

