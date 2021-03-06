﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PaginaInterna.Master"
    CodeBehind="SolicitaDocumentos.aspx.cs" Inherits="AuditoriaParlamentar.SolicitaDocumentos" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <div class="row">
            <div class="col-md-6">
                <div class="form-group">
                    <label class="control-label">CNPJ/CPF:</label>
                    <span runat="server" id="lblCNPJ" class="control-label show"></span>
                </div>
            </div>
            <div class="col-md-6">
                <div class="form-group">
                    <label class="control-label">Razão Social:</label>
                    <span runat="server" id="lblrazaoSocial" class="control-label show"></span>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <div class="form-group">
                    <label class="control-label">Selecione o Parlamentar:</label>
                    <asp:DropDownList ID="DropDownListParlamentar" runat="server" AutoPostBack="True"
                        OnSelectedIndexChanged="DropDownListParlamentar_SelectedIndexChanged" CssClass="form-control input-sm">
                    </asp:DropDownList>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <asp:TextBox ID="TextBoxTexto" runat="server" Rows="10" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <div class="form-group">
                    <label id="LabelSel" runat="server" class="control-label">Selecione os documentos que deseja solicitar e clique em Gerar Texto.</label>
                    <br />
                    <asp:Button ID="ButtonGerar" runat="server" Text="Gerar Texto" CssClass="btn btn-success btn-sm" OnClick="ButtonGerar_Click" />
                    <br />
                    <br />
                    <asp:HyperLink ID="HyperLinkCamara" runat="server" Target="_blank">Você poderá consultar se o documento está disponível no portal da Câmara.</asp:HyperLink>
                    <br />
                    <asp:HyperLink ID="HyperLinkDoc" runat="server" NavigateUrl="http://www2.camara.leg.br/participe/fale-conosco/?contexto=biblarq"
                        Target="_blank">Clique aqui para abrir uma solicitação no Fale Conosco da Câmara dos Deputados. Copie o texto abaixo e cole na mensagem da solicitação. Após a solicitação ser processada a Câmara enviará um e-mail com instruções de como fazer o pagamento. O custo é de R$ 0,15 centavos por folha.</asp:HyperLink>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <div class="table-responsive">
                    <asp:GridView ID="GridViewResultado" runat="server" AllowSorting="True" UseAccessibleHeader="true" OnRowDataBound="GridViewResultado_RowDataBound"
                        CssClass="table table-hover table-striped" GridLines="None" ShowFooter="True" OnSorting="GridViewResultado_Sorting">
                        <Columns>
                            <asp:TemplateField HeaderText="Selecionar">
                                <ItemTemplate>
                                    <center>
                                            <asp:CheckBox ID="CheckBoxSelecionar" runat="server" /></center>
                                </ItemTemplate>
                                <HeaderStyle Width="50px" />
                            </asp:TemplateField>
                        </Columns>
                        <RowStyle CssClass="cursor-pointer" />
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
