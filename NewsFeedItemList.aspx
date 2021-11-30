<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NewsFeedItemList.aspx.cs" Inherits="Unchained.NewsFeedItemList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script>
        function OnDelete() {
            if (confirm("Are you sure wants to delete?")) {
                return true;
            }
            else {
                return false;  
            }
        }
    </script>
    <table style="width:100%">
        <tr>
            <td style="text-align:left;">
                <asp:Label ID="lblHeading" runat="server" Text="News Feed List" Font-Bold="True" Font-Size="Large"></asp:Label>
                <br />
                 
                  </td>
            <td style="text-align:right">
              
            </td>
        </tr>
        <tr>
            <td>

                  <asp:LinkButton ID="btnAddNewFeed" runat="server" Font-Underline="True" OnClick="btnAddNewFeed_Click">Create New Feed</asp:LinkButton>
                 </td>
        </tr>
        <tr>
            <td>

                  <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="#009900"></asp:Label>
                 </td>
        </tr>
        <tr>
            <td  align="left">
                <div style="height:500px; overflow:auto">

                        <asp:GridView ID="GvNewsFeedItem" runat="server" AutoGenerateColumns="False" BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" ForeColor="Black" GridLines="Vertical" OnRowCommand="GvNewsFeedItem_RowCommand" Width="100%">
        <AlternatingRowStyle BackColor="#CCCCCC" />
        <Columns>

            <asp:BoundField DataField="FeedName" HeaderText="Feed Name">
            <HeaderStyle ForeColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="URL" HeaderText="Url">
            <HeaderStyle ForeColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="Weight" HeaderText="Weight">
            <HeaderStyle ForeColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="Notes" HeaderText="Notes">
            <HeaderStyle ForeColor="White" />
            </asp:BoundField>
         
 
             <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="btnEdit" runat="server" CommandArgument='<%# Bind("id") %>' CommandName="Edit" Font-Underline="true">Edit</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="btnDelete" OnClientClick="return OnDelete();" runat="server" CommandArgument='<%# Bind("id") %>' CommandName="Delete" Font-Underline="true">Delete</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            
        </Columns>
                            <EmptyDataTemplate>
                                <table cellspacing="0" cellpadding="3" rules="cols" id="MainContent_GvNewsFeedItem" style="color:Black;background-color:White;border-color:#999999;border-width:1px;border-style:Solid;width:100%;border-collapse:collapse;">
			<tbody><tr style="color:White;background-color:Black;font-weight:bold;">
				<th scope="col" style="color:White;">Feed Name</th><th scope="col" style="color:White;">Url</th><th scope="col" style="color:White;">Weight</th><th scope="col" style="color:White;">Notes</th><th scope="col">&nbsp;</th><th scope="col">&nbsp;</th>
			</tr><tr>
				<td colspan="6">Records not found</td>
			</tr>
		</tbody></table>
                            </EmptyDataTemplate>
        <FooterStyle BackColor="#CCCCCC" />
        <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
        <SortedAscendingCellStyle BackColor="#F1F1F1" />
        <SortedAscendingHeaderStyle BackColor="Gray" />
        <SortedDescendingCellStyle BackColor="#CAC9C9" />
        <SortedDescendingHeaderStyle BackColor="#383838" />
    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
    
</asp:Content>
