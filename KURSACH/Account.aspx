<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Account.aspx.cs" Inherits="KURSACH.Account" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Личный кабинет</h2>

    <asp:Label ID="WelcomeLabel" runat="server" Text="Добро пожаловать, " Font-Bold="True" Font-Size="Large"></asp:Label>
    <br /><br />

    <!-- Информация для пациента -->
    <asp:Panel ID="PatientInfoPanel" runat="server" Visible="false">
        <h3>Медицинская книжка</h3>
        <p><strong>Диагноз:</strong> <asp:Label ID="DiagnosisLabel" runat="server"></asp:Label></p>
        <p><strong>Лечение:</strong> <asp:Label ID="TreatmentLabel" runat="server"></asp:Label></p>
        <p><strong>Последний медицинский тест:</strong> <asp:Label ID="LastTestLabel" runat="server"></asp:Label></p>

        <h3>Записи на прием</h3>
        <asp:GridView ID="AppointmentsGrid" runat="server" AutoGenerateColumns="True" EmptyDataText="Вы не записаны на прием." />
        
        <h3>Медицинские тесты</h3>
        <asp:GridView ID="MedicalTestsGrid" runat="server" AutoGenerateColumns="True" EmptyDataText="Вы не проходили медицинские тесты." />

    </asp:Panel>

    <!-- Информация для других ролей, например для врачей или администраторов -->
    <asp:Panel ID="OtherRolesInfoPanel" runat="server" Visible="false">
        <h3>Информация для других ролей</h3>
        <p>Здесь будет информация для врачей и администраторов.</p>
    </asp:Panel>
</asp:Content>
