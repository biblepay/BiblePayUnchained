<%@ Page Title="Freedom Fighter News" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NewsView.aspx.cs" Inherits="Unchained.NewsView" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    
<style>
    .fetched-posts {
    font-family: inherit;
    font-size: 12px;
}
ul {
    list-style: disc;
    padding: 0px;
}
.fetched-posts li {
    list-style: none;
    background: #F7F7F7;
    padding: 5px 0px;
    margin-bottom: 0px;
    border-bottom: 1px solid gainsboro;
}
.fetched-posts a {
    color: #000;
    font-weight: 500;
}
.fetched-posts p {
    font-size: 12px;
    display:none;
}
    .CustomNew {
            display: block;
    text-overflow: ellipsis;
    word-wrap: break-word;
    overflow: hidden;
    max-height: 2.6em;
        padding-left: 16px;
        padding-right:10px;
        border-bottom: 1px solid;
        background-color: aliceblue;
    }
    .img {
       
    height: 300px;
    width: 410px;
    margin-left: 45px;
    border: 10px solid black;

    }
</style>
 

     <table>

        <tr>
            <td>  
                <img width='150px' height='100px' src="Images/news_icon3.png" />

            </td> <td>              <h1>Freedom Fighter News:</h1>  
            <td><img width='150px' height='100px' src="Images/news_icon3.png" />
        </tr>
    </table>
    <hr />

    <asp:Repeater ID="Repeater1" runat="server" >  
        <ItemTemplate> 
        <table> 
          <tr>
              <td style="vertical-align:top; text-align:left; border:1px solid black; width:500px">
                    <a target='_blank' href='<%#Eval("URL")%>'><h3 class='headline CustomNew'><%#Eval("Title")%></h3></a><br>
                  <img width='500px' height='300px' src='<%#Eval("ImageURL")%>' style="display:<%#Eval("Display")%>" class="img" />
                  <br>
                    <span class='headline'><%#Eval("Body")%></span><br><br>
                    <a target='_blank' href='<%#Eval("URL")%>' style='text-decoration: underline;margin-left: 10px;'>Read more </a><%#Eval("Body").ToString().Substring(0,10)%>....<br><br>
              </td>
               <td  style="vertical-align:top; text-align:left; border:1px solid black; width:500px">
                    <a target='_blank' href='<%#Eval("URLCol2")%>'><h3 class='headline CustomNew'><%#Eval("TitleCol2")%></h3></a><br>
                  <img width='500px' height='300px' src='<%#Eval("ImageURLCol2")%>'  style="display:<%#Eval("DisplayCol2")%>" class="img" />
                  <br>
                    <span class='headline'><%#Eval("BodyCol2")%></span><br><br>
                    <a target='_blank' href='<%#Eval("URLCol2")%>' style='text-decoration: underline;margin-left: 10px;'>Read more  </a><%#Eval("BodyCol2").ToString().Substring(0,10)%>....<br><br>
              </td>
         
          </tr>
        </table>
        </ItemTemplate>
    </asp:Repeater>

    <%--<asp:DataList ID="DataList1" runat="server" DataKeyField="id" RepeatDirection="Horizontal"
    EnableViewState="False">
        <ItemTemplate>
         <table> 
          <tr>
              <td>
                  <a target='_blank' href='<%#Eval("URL")%>'><h2 class='headline'><%#Eval("Title")%></h2></a><br>
                  <img width='100%' height='500px' src='<%#Eval("ImageURL")%>' />
                  <br>
                    <span class='headline'><%#Eval("Body")%></span><br><br>
                  <a target='_blank' href='<%#Eval("URL")%>' style='text-decoration: underline;'>Read more</a><br><br>
              </td>

         
          </tr>
        </table>
            </ItemTemplate>
        </asp:DataList>--%>
   <br />  
        <div style="text-align:center">  
            <asp:Repeater ID="Repeater2" runat="server" OnItemCommand="Repeater2_ItemCommand">  
                <ItemTemplate>  
                    <asp:LinkButton ID="lnkPage"  
                        Style="padding: 8px; margin: 2px; background: lightgray; border: solid 1px #666; color: black; font-weight: bold"  
                        CommandName="Page" CommandArgument="<%# Container.DataItem %>" runat="server" Font-Bold="True"><%# Container.DataItem %>  
                    </asp:LinkButton>  
                </ItemTemplate>  
            </asp:Repeater>  
        </div>  
 



</asp:Content>
