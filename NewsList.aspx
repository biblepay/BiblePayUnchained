<%@ Page Title="Unchained News" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NewsList.aspx.cs" Inherits="Unchained.NewsList" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <table>

        <tr>
            <td>  
                <img width='150px' height='100px' src="Images/news_icon3.png" />

            </td> <td>              <h1>Unchained News:</h1>  
            <td><img width='150px' height='100px' src="Images/news_icon3.png" />
        </tr>
    </table>
    <hr />


    <%=GetNews() %>
 </asp:Content>
