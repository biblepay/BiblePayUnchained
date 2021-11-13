<%@ Page Title="Message Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreateNewDocument.aspx.cs" Inherits="Unchained.CreateNewDocument" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">


    <link rel="stylesheet" href="/richtexteditor/rte_theme_default.css" />
	<script type="text/javascript" src="/richtexteditor/rte.js"></script>
	<script type="text/javascript" src='/richtexteditor/plugins/all_plugins.js'></script>

	<div class="text-center">
		<h2 class="display-4">Create New Document</h2>


		<input name="htmlcode" id="inp_htmlcode" type="hidden" />


		<div id="div_editor1" class="richtexteditor" style="width: 960px; margin: 0 auto;">
		</div>

		<script>
			var editor1 = new RichTextEditor(document.getElementById("div_editor1"));
			editor1.attachEvent("change", function () {
				document.getElementById("inp_htmlcode").value = editor1.getHTMLCode();
            });


			/*
            function loadWikiDocument(data) {
                editor1.setHTMLCode(data);
            }
			*/

		</script>
		

	</div>
		<div style="margin: 0 auto; padding: 100px;">
			<br />

			Title: <asp:TextBox ID="txtTitle" width="200px" runat="server"></asp:TextBox>
		    
			<br />

			Save As: <asp:TextBox ID="txtSaveAs" width="200px" runat="server"></asp:TextBox>
		    <asp:Button ID="btnSave" runat="server" onclick="btnSave_Click" Text="Save" />
			
			

        </div>
		 

 </asp:Content>
