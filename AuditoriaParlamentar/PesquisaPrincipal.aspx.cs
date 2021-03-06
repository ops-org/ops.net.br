﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using AuditoriaParlamentar.Classes;

namespace AuditoriaParlamentar
{
	public partial class PesquisaPrincipal : System.Web.UI.Page
	{
		Double mTotalGeral = 0;
		Boolean mAuthenticated;
		Int32 mIdShare = 0;

		const String GRUPO_DEPUTADO_FEDERAL = "Deputado Federal";
		const String GRUPO_SENADOR = "Senador";

		protected void Page_Load(object sender, EventArgs e)
		{
			GridViewResultado.PreRender += GridViewResultado_PreRender;
			mAuthenticated = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

			if (mAuthenticated)
			{
				if (System.Web.HttpContext.Current.User.IsInRole("REVISOR"))
				{
					ButtonShare.Visible = true;
				}
			}

			if (!IsPostBack)
			{
				//Chave para exportação do CSV
				ViewState["ChavePesquisa"] = Guid.NewGuid().ToString();

				DropDownListGrupo.Items.Add(new ListItem(GRUPO_DEPUTADO_FEDERAL, GRUPO_DEPUTADO_FEDERAL));
				DropDownListGrupo.Items.Add(new ListItem(GRUPO_SENADOR, GRUPO_SENADOR));

				if (Convert.ToString(Session["TipoPesquisa"]) == "SENADOR")
				{
					DropDownListGrupo.SelectedValue = GRUPO_SENADOR;
				}

				switch (DropDownListGrupo.SelectedValue)
				{
					case GRUPO_DEPUTADO_FEDERAL:
						Dados dados = new Dados();
						dados.CarregaValores(Cache, DropDownListParlamentar, DropDownListDespesa, DropDownListPartido);
						LabelUltimaAtualizacao.Text = "Última Atualização: " + dados.UltimaAtualizacao.ToString("dd/MM/yyyy");
						break;

					case GRUPO_SENADOR:
						DadosSenadores dadosSenadores = new DadosSenadores();
						dadosSenadores.CarregaValores(Cache, DropDownListParlamentar, DropDownListDespesa, DropDownListPartido);
						LabelUltimaAtualizacao.Text = "Última Atualização: " + dadosSenadores.UltimaAtualizacao.ToString("dd/MM/yyyy");
						break;
				}

				DropDownListPerido.Items.Add(new ListItem(Pesquisa.PERIODO_MES_ATUAL, Pesquisa.PERIODO_MES_ATUAL));
				DropDownListPerido.Items.Add(new ListItem(Pesquisa.PERIODO_MES_ANTERIOR, Pesquisa.PERIODO_MES_ANTERIOR));
				DropDownListPerido.Items.Add(new ListItem(Pesquisa.PERIODO_MES_ULT_4, Pesquisa.PERIODO_MES_ULT_4) { Selected = true });
				DropDownListPerido.Items.Add(new ListItem(Pesquisa.PERIODO_ANO_ATUAL, Pesquisa.PERIODO_ANO_ATUAL));
				DropDownListPerido.Items.Add(new ListItem(Pesquisa.PERIODO_ANO_ANTERIOR, Pesquisa.PERIODO_ANO_ANTERIOR));
				DropDownListPerido.Items.Add(new ListItem(Pesquisa.PERIODO_MANDATO_55, Pesquisa.PERIODO_MANDATO_55));
				DropDownListPerido.Items.Add(new ListItem(Pesquisa.PERIODO_MANDATO_54, Pesquisa.PERIODO_MANDATO_54));
				DropDownListPerido.Items.Add(new ListItem(Pesquisa.PERIODO_MANDATO_53, Pesquisa.PERIODO_MANDATO_53));
				DropDownListPerido.Items.Add(new ListItem(Pesquisa.PERIODO_INFORMAR, Pesquisa.PERIODO_INFORMAR));
				DropDownListPerido.Items.Add(new ListItem(Pesquisa.TUDO, Pesquisa.TUDO));

				DropDownListAgrupamento.Items.Add(new ListItem(Pesquisa.AGRUPAMENTO_PARLAMENTAR, Pesquisa.AGRUPAMENTO_PARLAMENTAR));
				DropDownListAgrupamento.Items.Add(new ListItem(Pesquisa.AGRUPAMENTO_DESPESA, Pesquisa.AGRUPAMENTO_DESPESA));
				DropDownListAgrupamento.Items.Add(new ListItem(Pesquisa.AGRUPAMENTO_FORNECEDOR, Pesquisa.AGRUPAMENTO_FORNECEDOR));
				DropDownListAgrupamento.Items.Add(new ListItem(Pesquisa.AGRUPAMENTO_PARTIDO, Pesquisa.AGRUPAMENTO_PARTIDO));
				DropDownListAgrupamento.Items.Add(new ListItem(Pesquisa.AGRUPAMENTO_UF, Pesquisa.AGRUPAMENTO_UF));
				DropDownListAgrupamento.Items.Add(new ListItem(Pesquisa.AGRUPAMENTO_DOCUMENTO, Pesquisa.AGRUPAMENTO_DOCUMENTO));

				DropDownListMesInicial.Items.Add(new ListItem("Jan", "01"));
				DropDownListMesInicial.Items.Add(new ListItem("Fev", "02"));
				DropDownListMesInicial.Items.Add(new ListItem("Mar", "03"));
				DropDownListMesInicial.Items.Add(new ListItem("Abr", "04"));
				DropDownListMesInicial.Items.Add(new ListItem("Maio", "05"));
				DropDownListMesInicial.Items.Add(new ListItem("Jun", "06"));
				DropDownListMesInicial.Items.Add(new ListItem("Jul", "07"));
				DropDownListMesInicial.Items.Add(new ListItem("Ago", "08"));
				DropDownListMesInicial.Items.Add(new ListItem("Set", "09"));
				DropDownListMesInicial.Items.Add(new ListItem("Out", "10"));
				DropDownListMesInicial.Items.Add(new ListItem("Nov", "11"));
				DropDownListMesInicial.Items.Add(new ListItem("Dez", "12"));

				DropDownListMesFinal.Items.Add(new ListItem("Jan", "01"));
				DropDownListMesFinal.Items.Add(new ListItem("Fev", "02"));
				DropDownListMesFinal.Items.Add(new ListItem("Mar", "03"));
				DropDownListMesFinal.Items.Add(new ListItem("Abr", "04"));
				DropDownListMesFinal.Items.Add(new ListItem("Maio", "05"));
				DropDownListMesFinal.Items.Add(new ListItem("Jun", "06"));
				DropDownListMesFinal.Items.Add(new ListItem("Jul", "07"));
				DropDownListMesFinal.Items.Add(new ListItem("Ago", "08"));
				DropDownListMesFinal.Items.Add(new ListItem("Set", "09"));
				DropDownListMesFinal.Items.Add(new ListItem("Out", "10"));
				DropDownListMesFinal.Items.Add(new ListItem("Nov", "11"));
				DropDownListMesFinal.Items.Add(new ListItem("Dez", "12"));

				DropDownListUF.Items.Add(new ListItem("Acre", "AC"));
				DropDownListUF.Items.Add(new ListItem("Alagoas", "AL"));
				DropDownListUF.Items.Add(new ListItem("Amapá", "AP"));
				DropDownListUF.Items.Add(new ListItem("Amazonas", "AM"));
				DropDownListUF.Items.Add(new ListItem("Bahia", "BA"));
				DropDownListUF.Items.Add(new ListItem("Ceará", "CE"));
				DropDownListUF.Items.Add(new ListItem("Distrito Federal", "DF"));
				DropDownListUF.Items.Add(new ListItem("Espírito Santo", "ES"));
				DropDownListUF.Items.Add(new ListItem("Goiás", "GO"));
				DropDownListUF.Items.Add(new ListItem("Maranhão", "MA"));
				DropDownListUF.Items.Add(new ListItem("Minas Gerais", "MG"));
				DropDownListUF.Items.Add(new ListItem("Mato Grosso", "MT"));
				DropDownListUF.Items.Add(new ListItem("Mato Grosso do Sul", "MS"));
				DropDownListUF.Items.Add(new ListItem("Pará", "PA"));
				DropDownListUF.Items.Add(new ListItem("Paraíba", "PB"));
				DropDownListUF.Items.Add(new ListItem("Paraná", "PR"));
				DropDownListUF.Items.Add(new ListItem("Pernambuco", "PE"));
				DropDownListUF.Items.Add(new ListItem("Piauí", "PI"));
				DropDownListUF.Items.Add(new ListItem("Rio de Janeiro", "RJ"));
				DropDownListUF.Items.Add(new ListItem("Rio Grande do Norte", "RN"));
				DropDownListUF.Items.Add(new ListItem("Rondônia", "RO"));
				DropDownListUF.Items.Add(new ListItem("Roraima", "RR"));
				DropDownListUF.Items.Add(new ListItem("Rio Grande do Sul", "RS"));
				DropDownListUF.Items.Add(new ListItem("Santa Catarina", "SC"));
				DropDownListUF.Items.Add(new ListItem("Sergipe", "SE"));
				DropDownListUF.Items.Add(new ListItem("São Paulo", "SP"));
				DropDownListUF.Items.Add(new ListItem("Tocantins", "TO"));

				//for (Int32 i = DateTime.Today.Year; i >= dados.MenorAno; i--)
				for (Int32 i = DateTime.Today.Year; i >= 2009; i--)
				{
					DropDownListAnoInicial.Items.Add(new ListItem(i.ToString(), i.ToString()));
					DropDownListAnoFinal.Items.Add(new ListItem(i.ToString(), i.ToString()));
				}

				//DropDownListPerido.Attributes.Add("onchange", "showHiddenPeriodo(this);");
			}
			else
			{
				if (Request["__EVENTTARGET"] == "ButtonExportar_Click")
				{
					if (ViewState["ChavePesquisa"] != null)
					{
						DataTable dtUltimaConsulta = HttpContext.Current.Session["AuditoriaUltimaConsulta" + ViewState["ChavePesquisa"].ToString()] as DataTable;
						if (dtUltimaConsulta != null)
						{
							DataTable dtExport = dtUltimaConsulta.Copy();
							dtExport.Columns.RemoveAt(0);
							CsvHelper.ExportarCSV(dtExport);
						}
					}
				}
			}
		}

		protected void Page_UnLoad(object sender, EventArgs e)
		{
			for (int i = 0; i < GridViewResultado.Rows.Count; i++)
			{
				for (Int32 j = 0; j < GridViewResultado.Rows[i].Cells.Count; j++)
				{
					GridViewResultado.Rows[i].Cells[j].Attributes.Remove("style");
				}
			}
		}

		private void GridViewResultado_PreRender(object sender, EventArgs e)
		{
			try
			{
				if (GridViewResultado.HeaderRow != null)
					GridViewResultado.HeaderRow.TableSection = TableRowSection.TableHeader;
			}
			catch (Exception ex)
			{
			}
		}

		protected void Page_LoadComplete(Object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				if (!string.IsNullOrEmpty(Request.QueryString["IdShare"]))
				{
					Int32.TryParse(Request.QueryString["IdShare"], out mIdShare);
				}
				else {
					if (!string.IsNullOrEmpty(Request.QueryString["periodo"]))
					{
						DropDownListPerido.SelectedValue = Request.QueryString["periodo"];
					}
					else if (DropDownListPerido.SelectedItem == null)
					{
						DropDownListPerido.SelectedValue = Pesquisa.PERIODO_MES_ULT_4;
					}

					SelecionaItemDDL(DropDownListGrupo, "cargo");
					SelecionaItemDDL(DropDownListPerido, "periodo");
					SelecionaItemDDL(DropDownListParlamentar, "parlamentar");
					SelecionaItemDDL(DropDownListDespesa, "despesa");
					SelecionaItemDDL(DropDownListUF, "uf");
					SelecionaItemDDL(DropDownListPartido, "partido");
					txtFornecedor.Text = Request.QueryString["fornecedor"];
					SelecionaItemDDL(DropDownListAgrupamento, "agrupamento");
				}

				Pesquisar();
			}
		}

		private void SelecionaItemDDL(DropDownList ddl, string qsKey)
		{
			if (!string.IsNullOrEmpty(Request.QueryString[qsKey]))
			{
				ddl.SelectedValue = Request.QueryString[qsKey];
			}
		}

		private void SelecionaItemDDL(ListBox ddl, string qsKey)
		{
			if (!string.IsNullOrEmpty(Request.QueryString[qsKey]))
			{
				ListItem item = ddl.Items.FindByValue(Request.QueryString[qsKey]);
				if (item != null)
				{
					ddl.ClearSelection();
					item.Selected = true;
				}
			}
		}

		protected void ButtonPesquisar_Click(object sender, EventArgs e)
		{
			Pesquisar();
		}

		private void Pesquisar()
		{
			if (mIdShare > 0)
			{
				ParametrosShare parametros = DbShare.Carregar(mIdShare);

				if (parametros == null)
					return;

				DropDownListGrupo.SelectedValue = parametros.Cargo;
				DropDownListAgrupamento.SelectedValue = parametros.Agrupamento;
				DropDownListPerido.SelectedValue = Pesquisa.PERIODO_INFORMAR;
				DropDownListParlamentar.SelectedItems(parametros.Parlamentares);
				DropDownListDespesa.SelectedItems(parametros.Despesas);
				txtFornecedor.Text = parametros.Fornecedores.Split('|')[0];
				DropDownListPartido.SelectedItems(parametros.Partidos);
				DropDownListUF.SelectedItems(parametros.Ufs);
				DropDownListMesInicial.SelectedValue = parametros.MesInicial.ToString("00");
				DropDownListAnoInicial.SelectedValue = parametros.AnoInicial.ToString("00");
				DropDownListMesFinal.SelectedValue = parametros.MesFinal.ToString("00");
				DropDownListAnoFinal.SelectedValue = parametros.AnoFinal.ToString("00");
			}

			if (CheckBoxSepararMes.Checked == true && DropDownListPerido.SelectedValue == Pesquisa.PERIODO_INFORMAR)
			{
				DateTime dt1 = new DateTime(Convert.ToInt32(DropDownListAnoInicial.SelectedValue), Convert.ToInt32(DropDownListMesInicial.SelectedValue), 1);
				DateTime dt2 = new DateTime(Convert.ToInt32(DropDownListAnoFinal.SelectedValue), Convert.ToInt32(DropDownListMesFinal.SelectedValue), 1);

				if ((Convert.ToInt32(DropDownListMesFinal.SelectedValue) - Convert.ToInt32(DropDownListMesInicial.SelectedValue) + 1) + 12 * (Convert.ToInt32(DropDownListAnoFinal.SelectedValue) - Convert.ToInt32(DropDownListAnoInicial.SelectedValue)) > 24)
				{
					Response.Write("<script>alert('Período informado é grande demais para separar os meses. Informo no máximo 24 meses.')</script>");
					return;
				}
			}

			//HiddenFieldGrupo.Value = DropDownListGrupo.SelectedValue;
			//HiddenFieldAgrupamentoAtual.Value = DropDownListAgrupamento.SelectedValue;

			var lstParlamentar = String.Join(",", DropDownListParlamentar.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value));
			var lstDespesa = String.Join(",", DropDownListDespesa.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value));
			var lstUF = String.Join(",", DropDownListUF.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value));
			var lstPartido = String.Join(",", DropDownListPartido.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value));

			switch (DropDownListGrupo.SelectedValue)
			{
				case GRUPO_DEPUTADO_FEDERAL:
					Pesquisa pesquisa = new Pesquisa();
					pesquisa.Carregar(GridViewResultado,
						HttpContext.Current.User.Identity.Name,
						"",
						DropDownListPerido.SelectedValue,
						DropDownListAgrupamento.SelectedValue,
						CheckBoxSepararMes.Checked,
						DropDownListAnoInicial.SelectedValue,
						DropDownListMesInicial.SelectedValue,
						DropDownListAnoFinal.SelectedValue,
						DropDownListMesFinal.SelectedValue,
						lstParlamentar,
						lstDespesa,
						txtFornecedor.Text,
						lstUF,
						lstPartido,
						ViewState["ChavePesquisa"].ToString());
					break;

				case GRUPO_SENADOR:
					PesquisaSenadores pesquisaSenadores = new PesquisaSenadores();
					pesquisaSenadores.Carregar(GridViewResultado,
						HttpContext.Current.User.Identity.Name,
						"",
						DropDownListPerido.SelectedValue,
						DropDownListAgrupamento.SelectedValue,
						CheckBoxSepararMes.Checked,
						DropDownListAnoInicial.SelectedValue,
						DropDownListMesInicial.SelectedValue,
						DropDownListAnoFinal.SelectedValue,
						DropDownListMesFinal.SelectedValue,
						lstParlamentar,
						lstDespesa,
						txtFornecedor.Text,
						lstUF,
						lstPartido,
						ViewState["ChavePesquisa"].ToString());
					break;
			}

			Session["pesquisa0"] = GridViewResultado.DataSource;
			Session["SortDirection0"] = "DESC";
			Session["SortExpression0"] = "Valor Total";

			if (GridViewResultado.Rows.Count == 1000)
			{
				LabelMaximo.InnerText = "O resultado está limitado a 1.000 registros para evitar sobrecarga.";
				LabelMaximo.Visible = true;
			}
			else
			{
				LabelMaximo.InnerText = "";
				LabelMaximo.Visible = false;
			}
		}

		protected void GridViewResultado_Sorting(object sender, GridViewSortEventArgs e)
		{
			//Retrieve the table from the session object.
			DataTable dt = Session["pesquisa0"] as DataTable;

			if (dt != null)
			{
				//Sort the data.
				dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
				GridViewResultado.DataSource = Session["pesquisa0"];
				GridViewResultado.DataBind();
			}
		}

		private string GetSortDirection(string column)
		{

			// By default, set the sort direction to ascending.
			string sortDirection = "DESC";

			// Retrieve the last column that was sorted.
			string sortExpression = Session["SortExpression0"] as string;

			if (sortExpression != null)
			{
				// Check if the same column is being sorted.
				// Otherwise, the default value can be returned.
				if (sortExpression == column)
				{
					string lastDirection = Session["SortDirection0"] as string;
					if ((lastDirection != null) && (lastDirection == "DESC"))
					{
						sortDirection = "ASC";
					}
				}
			}

			Session["SortDirection0"] = sortDirection;
			Session["SortExpression0"] = column;

			return sortDirection;
		}

		protected void GridViewResultado_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType != DataControlRowType.EmptyDataRow)
			{
				e.Row.Cells[1].Visible = false;
				Int32 inicioColunaValores = 0;

				switch (DropDownListAgrupamento.SelectedValue)
				{
					case Pesquisa.AGRUPAMENTO_PARLAMENTAR:
						inicioColunaValores = 7;
						e.Row.Cells[6].Visible = false; //url
						break;

					case Pesquisa.AGRUPAMENTO_FORNECEDOR:
						inicioColunaValores = 8;

						if (mAuthenticated == false)
						{
							e.Row.Cells[Pesquisa.INDEX_COLUNA_AUDITEI + 1].Visible = false;
						}

						break;

					case Pesquisa.AGRUPAMENTO_DESPESA:
					case Pesquisa.AGRUPAMENTO_PARTIDO:
					case Pesquisa.AGRUPAMENTO_UF:
						inicioColunaValores = 3;
						break;

					case Pesquisa.AGRUPAMENTO_DOCUMENTO:
						inicioColunaValores = 11;

						e.Row.Cells[7].Visible = false;
						e.Row.Cells[8].Visible = false;
						e.Row.Cells[9].Visible = false;
						e.Row.Cells[10].Visible = false;

						break;

				}

				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					for (Int32 i = inicioColunaValores; i < e.Row.Cells.Count; i++)
					{
						Double valor;

						if (Double.TryParse(e.Row.Cells[i].Text, out valor))
							e.Row.Cells[i].Text = Convert.ToDouble(valor).ToString("N2");
						else
							e.Row.Cells[i].Text = "0,00";

						//e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
					}

					Button buttonAuditar = (Button)e.Row.FindControl("ButtonAuditar");

					switch (DropDownListAgrupamento.SelectedValue)
					{
						case Pesquisa.AGRUPAMENTO_FORNECEDOR:
							buttonAuditar.Text = "Auditar";

							if (mAuthenticated == true)
							{
								buttonAuditar.Visible = true;

								if (HttpUtility.HtmlDecode(e.Row.Cells[1].Text).Trim() == "")
								{
									buttonAuditar.OnClientClick = "window.parent.AlertaSemCnpj();return false;";
								}
								else
								{
									buttonAuditar.OnClientClick = "af(this, 2, 1);UpdateGridView(" + (e.Row.RowIndex + 1) + "," + Pesquisa.INDEX_COLUNA_AUDITEI.ToString() + ",'Sim');return false;";

									switch (DropDownListGrupo.SelectedValue)
									{
										case GRUPO_DEPUTADO_FEDERAL:
											e.Row.Cells[3].Text = string.Format("<a href=\"javascript:;\" onclick=\"af(this, 2, 1);\">{0}</a>", e.Row.Cells[3].Text);
											break;
										case GRUPO_SENADOR:
											break;
									}
								}
							}
							else
							{
								buttonAuditar.Visible = false;
							}

							e.Row.Cells[2].ControlStyle.CssClass = "maxWidthGrid";

							try { e.Row.Cells[6].Text = Convert.ToDateTime(e.Row.Cells[6].Text).ToString("dd/MM/yyyy"); }
							catch { }

							break;

						case Pesquisa.AGRUPAMENTO_PARLAMENTAR:
							switch (DropDownListGrupo.SelectedValue)
							{
								case GRUPO_DEPUTADO_FEDERAL:
									e.Row.Cells[2].Text = string.Format("<a href=\"javascript:;\" onclick=\"adf(this, {0});\">{1}</a>", e.Row.Cells[1].Text, e.Row.Cells[2].Text);
									//buttonAuditar.OnClientClick = "window.open('DeputadoFederal.aspx?id=" + e.Row.Cells[1].Text + "');return false;";
									buttonAuditar.Visible = false;
									break;
								case GRUPO_SENADOR:
									//TODO: Criar pagina de perfil e link
									buttonAuditar.Text = "Site";
									buttonAuditar.OnClientClick = "window.open('" + e.Row.Cells[5].Text + "');return false;";
									break;
							}

							break;

						case Pesquisa.AGRUPAMENTO_DOCUMENTO:
							buttonAuditar.Visible = false;

							try { e.Row.Cells[3].Text = Convert.ToDateTime(e.Row.Cells[3].Text).ToString("dd/MM/yyyy"); }
							catch { }

							long refDoc;

							if (Int64.TryParse(e.Row.Cells[10].Text, out refDoc))
							{
								if (refDoc > 0)
								{
									HyperLink url = new HyperLink();
									url.NavigateUrl = "http://www.camara.gov.br/cota-parlamentar/documentos/publ/" + e.Row.Cells[8].Text + "/" + e.Row.Cells[9].Text + "/" + e.Row.Cells[10].Text + ".pdf";
									url.Target = "_blank";
									url.Text = e.Row.Cells[2].Text;
									e.Row.Cells[2].Controls.Clear();
									e.Row.Cells[2].Controls.Add(url);
								}
							}

							switch (DropDownListGrupo.SelectedValue)
							{
								case GRUPO_DEPUTADO_FEDERAL:
									e.Row.Cells[4].Text = string.Format("<a href=\"javascript:;\" onclick=\"af(this, 3, 4);\">{0}</a>", e.Row.Cells[4].Text);
									e.Row.Cells[6].Text = string.Format("<a href=\"javascript:;\" onclick=\"adf(this, {0})\">{1}</a>", e.Row.Cells[7].Text, e.Row.Cells[6].Text);
									break;
								case GRUPO_SENADOR:
									//TODO: Criar pagina de perfil e link
									break;
							}

							break;

						default:
							buttonAuditar.Visible = false;
							break;
					}

					mTotalGeral += Convert.ToDouble(e.Row.Cells[e.Row.Cells.Count - 1].Text);
				}
				//else if (e.Row.RowType == DataControlRowType.Header)
				//{
				//    for (Int32 i = inicioColunaValores; i < e.Row.Cells.Count; i++)
				//        e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
				//}
				else if (e.Row.RowType == DataControlRowType.Footer)
				{
					e.Row.Cells[e.Row.Cells.Count - 1].Text = mTotalGeral.ToString("N2");
					//e.Row.Cells[e.Row.Cells.Count - 1].HorizontalAlign = HorizontalAlign.Right;
				}
			}
		}

		protected void DropDownListGrupo_SelectedIndexChanged(object sender, EventArgs e)
		{
			DropDownListParlamentar.Items.Clear();
			DropDownListDespesa.Items.Clear();
			DropDownListPartido.Items.Clear();

			switch (DropDownListGrupo.SelectedValue)
			{
				case GRUPO_DEPUTADO_FEDERAL:
					Dados dados = new Dados();
					dados.CarregaValores(Cache, DropDownListParlamentar, DropDownListDespesa, DropDownListPartido);
					LabelUltimaAtualizacao.Text = "Última Atualização: " + dados.UltimaAtualizacao.ToString("dd/MM/yyyy");
					break;

				case GRUPO_SENADOR:
					DadosSenadores dadosSenadores = new DadosSenadores();
					dadosSenadores.CarregaValores(Cache, DropDownListParlamentar, DropDownListDespesa, DropDownListPartido);
					LabelUltimaAtualizacao.Text = "Última Atualização: " + dadosSenadores.UltimaAtualizacao.ToString("dd/MM/yyyy");
					break;
			}
		}

		protected void ButtonShare_Click(object sender, EventArgs e)
		{
			if (DropDownListPerido.SelectedValue != Pesquisa.PERIODO_INFORMAR)
			{
				Response.Write("<script>alert('Informe o período com mês e ano.')</script>");
				return;
			}

			DataTable dt = Session["pesquisa0"] as DataTable;

			if (dt == null || dt.Rows.Count == 0)
			{
				Response.Write("<script>alert('Faça a pesquisa.')</script>");
				return;
			}

			ParametrosShare parametros = new ParametrosShare();
			parametros.Cargo = DropDownListGrupo.SelectedValue;
			parametros.Agrupamento = DropDownListAgrupamento.SelectedValue;
			parametros.Parlamentares = DropDownListParlamentar.SelectedItems();
			parametros.Despesas = DropDownListDespesa.SelectedItems();
			parametros.Fornecedores = txtFornecedor.Text;
			parametros.Partidos = DropDownListPartido.SelectedItems();
			parametros.Ufs = DropDownListUF.SelectedItems();
			parametros.MesInicial = Convert.ToInt32(DropDownListMesInicial.SelectedValue);
			parametros.AnoInicial = Convert.ToInt32(DropDownListAnoInicial.SelectedValue);
			parametros.MesFinal = Convert.ToInt32(DropDownListMesFinal.SelectedValue);
			parametros.AnoFinal = Convert.ToInt32(DropDownListAnoFinal.SelectedValue);

			DbShare.Incluir(parametros);

			LabelMaximo.InnerText = "http://ops.net.br/PesquisaInicio.aspx?IdShare=" + parametros.Id.ToString();
			LabelMaximo.Visible = true;
		}
	}
}
