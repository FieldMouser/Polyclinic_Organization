<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="KURSACH.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Войти:</h1>

    <asp:Label ID="Email_lbl" runat="server" Text="Email"></asp:Label>
    <asp:TextBox ID="Email_tb" runat="server" Placeholder="Введите ваш email"></asp:TextBox>
    <br />

    <asp:Label ID="Password_lbl" runat="server" Text="Пароль"></asp:Label>
    <asp:TextBox ID="Password_tb" runat="server" TextMode="Password" Placeholder="Введите ваш пароль"></asp:TextBox>
    <br />
    <asp:Button ID="Login_btn" runat="server" Text="Войти" OnClick="Login_btn_Click" />

    <br />
    <br />
    <p>Еще на зарегистрировали аккаунт? <asp:HyperLink ID="Register_lnk" runat="server" NavigateUrl="~/Registration.aspx">Зарегистрироваться</asp:HyperLink></p>

    <asp:Label ID="Error_lbl" runat="server" ForeColor="Red" Visible="false"></asp:Label>



</asp:Content>
