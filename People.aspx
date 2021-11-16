<%@ Page Title="View Person" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="People.aspx.cs" Inherits="Unchained.People" %>

<%@ Register TagPrefix="BBP" Namespace="BiblePayPaginator" Assembly="BiblePayPaginator" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/pages/peopledirectory.css" rel="stylesheet" />
   <input type="hidden" id="hfPostback" />
     <asp:UpdatePanel runat="server" ID="UpdatePanel6">
        <ContentTemplate>


            <script>
              function search(ele) {
                  if (event.key === 'Enter') {
$('#people-directory').addClass('loading');
                      document.getElementById('<%= btnSearch.ClientID %>').click();
                      ele.value = "";
                  }
              }
            </script>

            <div class="inner-form">
                <div class="input-field">
                    <div class="choices" data-type="text" aria-haspopup="true" aria-expanded="false" dir="ltr">
                        <div class="choices__inner">
                            <asp:TextBox CssClass="choices__input" ID="txtSearch" runat="server" onkeydown="search(this);return (event.keyCode!=13);" autocapitalize="none" role="textbox" placeholder="Type to search..."></asp:TextBox>
                        </div>
                    </div>
                    <asp:Button ID="btnSearch" CssClass="btn-search" runat="server" Text="Search" UseSubmitBehavior="true" OnClick="btnSearch_Click"></asp:Button>
                </div>
            </div>



            <div class="row position-relative" id="people-directory">
                <div class="loader1">
                    <i class="fa fa-sun fa-spin fa-4x"></i>
                </div>
                <%=GetPeople()%>
            </div>

            <BBP:Paginator ID="paginator1" runat="server"></BBP:Paginator>

            <script>
 $('body').on('click', '.pagination a', function () {
$('#people-directory').addClass('loading');
});
            </script>
        </ContentTemplate>

    </asp:UpdatePanel>

</asp:Content>
