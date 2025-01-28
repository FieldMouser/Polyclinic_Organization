<%@ Page Title="Регистрация" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="KURSACH.Registration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Регистрация:</h1>

    <asp:Label ID="Full_Name_lbl" runat="server" Text="Введите ваше ФИО:"></asp:Label>
    <br />
    <asp:TextBox ID="Full_Name_tb" runat="server" Placeholder="Иванов Иван Иванович"></asp:TextBox>
    <br />
    <br />

    <asp:Label ID="Email_lbl" runat="server" Text="Введите ваш email:"></asp:Label>
    <br />
    <asp:TextBox ID="Email_tb" runat="server" Placeholder="ivanov@example.com"></asp:TextBox>
    <br />
    <br />

    <asp:Label ID="Password_lbl" runat="server" Text="Создайте пароль:"></asp:Label>
    <br />
    <asp:TextBox ID="Password_tb" runat="server" TextMode="Password" Placeholder="Введите ваш пароль"></asp:TextBox>
    <br />

    <asp:Label ID="Password_repeat_lbl" runat="server" Text="Подтвердите пароль:"></asp:Label>
    <br />
    <asp:TextBox ID="Password_repeat_tb" runat="server" TextMode="Password" Placeholder="Подтвердите пароль"></asp:TextBox>
    <br />
    <br />

    <asp:Label ID="Address_lbl" runat="server" Text="Введите ваш адрес:"></asp:Label>
    <br />
    <asp:TextBox ID="Address_tb" runat="server" Placeholder="Ваш адрес"></asp:TextBox>
    <br />
    <br />

    <asp:Label ID="Phone_lbl" runat="server" Text="Введите ваш телефон:"></asp:Label>
    <br />
    <asp:TextBox ID="Phone_tb" runat="server" Placeholder="Ваш телефон"></asp:TextBox>
    <br />
    <br />

    <asp:Button ID="Register_btn" runat="server" Text="Зарегистрироваться" OnClick="Register_btn_Click" />

    <br />
    <br />
    <p>Уже есть аккаунт? <asp:HyperLink ID="Login_lnk" runat="server" NavigateUrl="~/Login.aspx">Войти</asp:HyperLink></p>
</asp:Content>
