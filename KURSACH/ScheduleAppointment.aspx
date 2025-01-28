<%@ Page Title="Запись на прием" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ScheduleAppointment.aspx.cs" Inherits="KURSACH.ScheduleAppointment" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Записаться на прием</h2>

    <p>Выберите врача и удобное время:</p>

    <!-- Список врачей -->
    <asp:DropDownList ID="DoctorDropDown" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DoctorDropDown_SelectedIndexChanged">
        <asp:ListItem Text="Выберите врача" Value="0"></asp:ListItem>
    </asp:DropDownList>

    <!-- Список доступных времени -->
    <asp:DropDownList ID="TimeDropDown" runat="server" AutoPostBack="true">
        <asp:ListItem Text="Выберите время" Value="0" />
    </asp:DropDownList>

    <!-- Календарь для выбора даты -->
    <p>Выберите дату для записи:</p>
    <asp:Calendar ID="AppointmentCalendar" runat="server" OnSelectionChanged="AppointmentCalendar_SelectionChanged" />

    <p>Введите вашу жалобу или причину визита:</p>

    <!-- Текстовое поле для жалобы -->
    <asp:TextBox ID="ComplaintTextBox" runat="server" TextMode="MultiLine" Rows="4" Columns="50" placeholder="Введите жалобу"></asp:TextBox>

    <br /><br />

    <!-- Кнопка для записи на прием -->
    <asp:Button ID="BookAppointmentButton" runat="server" Text="Записаться на прием" OnClick="BookAppointmentButton_Click" />

</asp:Content>
