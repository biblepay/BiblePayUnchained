<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Unchained._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script>
        //Array of images which you want to show: Use path you want.
        var images = new Array('../Images/jc2.png', '../Images/jc3.jpg');
        var nextimage = 0;
        var slide = 0;
        doSlideshow();
function doSlideshow()
{
    if(nextimage>=images.length){nextimage=0;}
    $('.global-header').css('background-image', 'url("' + images[nextimage++] + '")');
    slide++;
   
    if (slide == 1) {
        setTimeout(doSlideshow, 777);
    }
    else {
        setTimeout(doSlideshow, 24000);
    }
}
</script>

    <div style="text-align:center">
      <h3>     Welcome to BiblePay's Decentralized Web    </h3>
    </div>
    <div class="container global-header" style="min-width:50%;min-height:200px;width:95%;height:390px"></div>
  
    <div id="AboutBiblePay1">
        <h4>About BiblePay Unchained:</h4>
        <ul>
            <li>Database operations on this server are done through DSQL (Decentralized SQL).  Metadata is stored in the chain in DSQL records, while data is stored offchain (in STORJ).</li>

            <li>We support rapid fire changes to the database.   </li>

        <li>This technology is decentralized meaning you can use it too, by consuming the BiblePay DLL.  What is the BiblePay DLL?  
            The BiblePay DLL is a Microsoft .NET DLL that incorporates our biblepay classes giving the ability to store files, videos, and data and host an asp.net web solution around the DLL.
            It makes it easy to provide a blockchain solution for your intranet or internet.
            </li>
        <li>What can the USERS do with this?  Social media (decentralized competitors to youtube, twitter, facebook) and a Christian Dashboard with gospel features.
            <br />
            Demo Examples: 
            A channel for each user, and channel for each church.  Tithing from a video.  Merchandise and Pet shop.

            <br />
        
        </li>
            <li>What can a developer write by consuming this DLL or what decentralized sites can be written?  
                Things that are hard to do on the blockchain but made easy.

                <br />Widgets that extend Gods Kingdom for our Christian Dashboard.  
                For example, Widgets that Teach the gospel to the masses.  
                <br />   
                Each widget has the full potential of the entire c# library plus blockchain interfaces!</li>

        </ul>

     </div>


</asp:Content>
