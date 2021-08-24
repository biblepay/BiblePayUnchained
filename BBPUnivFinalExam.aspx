<%@ Page Title="BiblePay University - Final Exam" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="BBPUnivFinalExam.aspx.cs" Inherits="Unchained.BBPUnivFinalExam" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
  
 

     <h3>BiblePay University - Final Exam - <asp:Label ID="lblTitle" runat="server" Text=""></asp:Label>                    &nbsp;  <small>v1.2</small></h3>

    <table>

        <tr>
            <td>        <asp:Label ID="lblQN" runat="server" Text="Question No:"></asp:Label>            </td>
            <td>       <asp:TextBox ID="txtQuestionNo" width="400px" type="number" runat="server"></asp:TextBox></td>     

        </tr>
         
        <tr>
            <td><asp:Label ID="lblQ" runat="server" Text="Question:"></asp:Label></td>
            <td>        <asp:TextBox ID="txtQuestion" runat="server" TextMode="MultiLine"  Rows="10" style="width: 900px">        </asp:TextBox></td>
  </tr>

    <tr>
        <td>        <asp:Label ID="lblA1" runat="server" Text="Your Answer:"></asp:Label></td>
        <td>  

<div>
                     <input runat="server" type="radio" id="radioAnswerA" name="radioAnswer" value="A"><asp:Label ID="lblA" runat="server" Text=""></asp:Label>
    <br />

                     <input runat="server" type="radio" id="radioAnswerB" name="radioAnswer"  value="B"><asp:Label ID="lblB" runat="server" Text=""></asp:Label>
    <br />

                     <input runat="server" type="radio" id="radioAnswerC" name="radioAnswer"  value="C"><asp:Label ID="lblC" runat="server" Text=""></asp:Label>
    <br />

                     <input runat="server" type="radio" id="radioAnswerD" name="radioAnswer"  value="D"><asp:Label ID="lblD" runat="server" Text=""></asp:Label>
                <br />
                     <asp:Label ID="lblGrade" runat="server" Text=""></asp:Label>
     </div>
            
        </td></tr>

    
        <tr>
            <td>        <asp:Label ID="lblComments" runat="server" Text="Comments/Answer:"></asp:Label></td>

            <td>        <asp:TextBox ID="txtAnswer" runat="server" TextMode="MultiLine"  Rows="10" style="width: 900px">        </asp:TextBox></td>

        </tr>

        </table>


        <asp:Button ID="btnBack" runat="server" Text="Back" OnClick="btnBack_Click" />
        <asp:Button ID="btnNext" runat="server" Text="Next" OnClick="btnNext_Click" />
        <asp:Button ID="btnSwitch" runat="server" Text="Switch" OnClick="btnSwitch_Click" />
        <asp:Button ID="btnGrade" runat="server" Text="Grade" OnClick="btnGrade_Click" />

   <br />



    <asp:Timer ID="Timer1" runat="server" Interval="5000" ontick="Timer1_Tick">
    </asp:Timer> 

<asp:UpdatePanel ID="UpdatePanel1"
    runat="server">
    <ContentTemplate>

        <table width="100%">

    <tr>
        <td width="70%"> 
            <asp:Label ID="lblInfo" runat="server" Text=""></asp:Label>
    </td><td>
     <asp:Label ID="lblElapsed" runat="server" Text=""></asp:Label>
      </td>  </tr>

        </table>

    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick">
        </asp:AsyncPostBackTrigger>
    </Triggers>
</asp:UpdatePanel>







</asp:Content>
