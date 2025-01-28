<%@ Page Title="Администрирование" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountAdmin.aspx.cs" Inherits="KURSACH.AccountAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Личный кабинет администратора</h1>

    <!-- Таблица с пользователями -->
    <h3>Список пользователей</h3>
    <asp:GridView ID="UsersGridView" runat="server" OnRowDeleting="UsersGridView_RowDeleting" AutoGenerateColumns="False" CssClass="table table-striped" 
        OnRowCommand="UsersGridView_RowCommand">
        <Columns>
            <asp:BoundField DataField="full_name" HeaderText="ФИО" SortExpression="full_name" />
            <asp:BoundField DataField="email" HeaderText="Email" SortExpression="email" />
            <asp:BoundField DataField="role" HeaderText="Роль" SortExpression="role" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Button ID="DeleteButton" runat="server" CommandName="Delete" CommandArgument='<%# Eval("id") %>' Text="Удалить" CssClass="btn btn-danger" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>


    <br />

    <!-- Форма для добавления нового пользователя -->
    <h3>Добавить нового доктора или администратора</h3>
    <asp:DropDownList ID="RoleDropDown" runat="server" CssClass="form-control">
        <asp:ListItem Text="Выберите роль" Value="" />
        <asp:ListItem Text="Доктор" Value="Doctor" />
        <asp:ListItem Text="Администратор" Value="Administrator" />
    </asp:DropDownList>
    <br /><br />

    <asp:TextBox ID="FullNameTextBox" runat="server" CssClass="form-control" Placeholder="ФИО" />
    <br />
    <asp:TextBox ID="EmailTextBox" runat="server" CssClass="form-control" Placeholder="Email" />
    <br />
    <asp:TextBox ID="PasswordTextBox" runat="server" CssClass="form-control" TextMode="Password" Placeholder="Пароль" />
    <br />
    <asp:Button ID="AddUserButton" runat="server" Text="Добавить пользователя" CssClass="btn btn-primary" OnClick="AddUserButton_Click" />

</asp:Content>
