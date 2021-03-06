﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using MySql.Data.MySqlClient;
using System.Web.UI.WebControls;
using System.Data;
using System.Threading;
using AuditoriaParlamentar.Classes;

namespace AuditoriaParlamentar.Classes
{
	public class Pesquisa
    {
        public const String PERIODO_MES_ATUAL = "Mês Atual";
        public const String PERIODO_MES_ANTERIOR = "Mês Anterior";
        public const String PERIODO_MES_ULT_4 = "Últimos 4 Meses";
        public const String PERIODO_ANO_ATUAL = "Ano Atual";
        public const String PERIODO_ANO_ANTERIOR = "Ano Anterior";
        public const String PERIODO_MANDATO_53 = "Legislatura 2007-2011";
        public const String PERIODO_MANDATO_54 = "Legislatura 2011-2015";
        public const String PERIODO_MANDATO_55 = "Legislatura 2015-2019";
        public const String PERIODO_INFORMAR = "Informar Período";
        public const String TUDO = "Tudo";
        public const String AGRUPAMENTO_PARLAMENTAR = "Por Parlamentar";
        public const String AGRUPAMENTO_FORNECEDOR = "Por Fornecedor";
        public const String AGRUPAMENTO_DESPESA = "Por Despesa";
        public const String AGRUPAMENTO_UF = "Por UF (Parlamentar)";
        public const String AGRUPAMENTO_PARTIDO = "Por Partido";
        public const String AGRUPAMENTO_DOCUMENTO = "Por Documento";
        public const String CARGO_DEPUTADO_FEDERAL = "Deputado Federal";
        public const String CARGO_SENADOR = "Senador";

        public const Int32 INDEX_COLUNA_AUDITEI = 4;

        public static String AnoMesIni { get; set; }
        public static String AnoMesFim { get; set; }

        public void Carregar(GridView grid,
            String userName, String documento, String periodo, String agrupamento, Boolean separarMes,
            String anoIni, String mesIni, String anoFim, String mesFim,
            string lstParlamentar, string lstDespesa, string lstFornecedor, string lstUF, string lstPartido, string ChavePesquisa)
        {
            using (Banco banco = new Banco())
            {
                StringBuilder sqlCmd = new StringBuilder();
                StringBuilder sqlCampos = new StringBuilder();
                StringBuilder sqlFrom = new StringBuilder();
                StringBuilder sqlWhere = new StringBuilder();

                sqlFrom.Append("   FROM lancamentos");
                sqlWhere.Append(" WHERE lancamentos.ideCadastro > 0");

                switch (agrupamento)
                {
                    case AGRUPAMENTO_PARLAMENTAR:
                        sqlCampos.Append("   SELECT lancamentos.ideCadastro AS codigo,");
                        sqlCampos.Append("          lancamentos.txNomeParlamentar AS agrupamento,");
                        sqlCampos.Append("          lancamentos.sgUF,");
                        sqlCampos.Append("          lancamentos.sgPartido,");
                        sqlCampos.Append("          COUNT(1) As TotalNotas,");
                        //sqlCampos.Append("          COUNT(DISTINCT IFNULL(lancamentos.txtnumero, lancamentos.id)) As TotalNotas,");
                        sqlCampos.Append("          '' AS url,");

                        break;

                    case AGRUPAMENTO_DESPESA:
                        sqlCampos.Append("   SELECT lancamentos.numSubCota AS codigo,");
                        sqlCampos.Append("          despesas.txtDescricao AS agrupamento,");

                        sqlFrom.Append(", despesas");
                        sqlWhere.Append(" AND lancamentos.numSubCota = despesas.numSubCota");

                        break;

                    case AGRUPAMENTO_FORNECEDOR:
                        sqlCampos.Append("   SELECT lancamentos.txtCNPJCPF AS codigo,");
                        sqlCampos.Append("          SUBSTRING(IFNULL(fornecedores.txtbeneficiario, lancamentos.txtbeneficiario), 1, 50) AS txtbeneficiario,");
                        sqlCampos.Append("          lancamentos.txtCNPJCPF AS agrupamento,");
                        sqlCampos.Append("          fornecedores.uf,");
                        sqlCampos.Append("          CASE WHEN UserName IS NULL THEN '' ELSE 'Sim' END AS auditei,");
                        sqlCampos.Append("          fornecedores.DataUltimaNotaFiscal,");
                        sqlCampos.Append("          CASE WHEN doador = 1 THEN 'Sim' ELSE '' END AS doador,");

                        sqlFrom.Append(" LEFT JOIN fornecedores");
                        sqlFrom.Append("        ON fornecedores.txtCNPJCPF = lancamentos.txtCNPJCPF");
                        sqlFrom.Append(" LEFT JOIN fornecedores_visitado");
                        sqlFrom.Append("        ON fornecedores_visitado.txtCNPJCPF = lancamentos.txtCNPJCPF");
                        sqlFrom.Append("       AND fornecedores_visitado.UserName   = @UserName");

                        banco.AddParameter("UserName", System.Web.HttpContext.Current.User.Identity.Name);

                        break;

                    case AGRUPAMENTO_PARTIDO:
                        sqlCampos.Append("   SELECT lancamentos.sgPartido AS codigo,");
                        sqlCampos.Append("          lancamentos.sgPartido AS agrupamento,");

                        break;

                    case AGRUPAMENTO_UF:
                        sqlCampos.Append("   SELECT lancamentos.sgUF AS codigo,");
                        sqlCampos.Append("          lancamentos.sgUF AS agrupamento,");

                        break;

                    case AGRUPAMENTO_DOCUMENTO:
                        sqlCampos.Append("   SELECT CONCAT(lancamentos.txtNumero, '|', lancamentos.txtCNPJCPF) AS codigo,");
                        sqlCampos.Append("          lancamentos.txtNumero,");
                        sqlCampos.Append("          lancamentos.datEmissao,");
                        sqlCampos.Append("          lancamentos.txtCNPJCPF AS agrupamento,");
                        sqlCampos.Append("          SUBSTRING(IFNULL(fornecedores.txtbeneficiario, lancamentos.txtbeneficiario), 1, 50) AS txtbeneficiario,");
                        sqlCampos.Append("          lancamentos.txNomeParlamentar,");
                        sqlCampos.Append("          parlamentares.ideCadastro,");
                        sqlCampos.Append("          parlamentares.nuDeputadoId,");
								sqlCampos.Append("          lancamentos.numano,");
                        sqlCampos.Append("          lancamentos.ideDocumento,");

                        sqlFrom.Append(" LEFT JOIN fornecedores");
                        sqlFrom.Append("        ON fornecedores.txtCNPJCPF = lancamentos.txtCNPJCPF");
                        sqlFrom.Append(" LEFT JOIN parlamentares");
                        sqlFrom.Append("        ON parlamentares.ideCadastro = lancamentos.ideCadastro");

                        separarMes = false;

                        break;

                }

                DateTime dataIni = DateTime.Today;
                DateTime dataFim = DateTime.Today;

                switch (periodo)
                {
                    case PERIODO_MES_ATUAL:
                        sqlWhere.Append(" AND lancamentos.anoMes = @anoMes");
                        banco.AddParameter("anoMes", dataIni.ToString("yyyyMM"));
                        break;

                    case PERIODO_MES_ANTERIOR:
                        dataIni = dataIni.AddMonths(-1);
                        dataFim = dataIni.AddMonths(-1);
                        sqlWhere.Append(" AND lancamentos.anoMes = @anoMes");
                        banco.AddParameter("anoMes", dataIni.ToString("yyyyMM"));
                        break;

                    case PERIODO_MES_ULT_4:
                        dataIni = dataIni.AddMonths(-3);
                        sqlWhere.Append(" AND lancamentos.anoMes >= @anoMes");
                        banco.AddParameter("anoMes", dataIni.ToString("yyyyMM"));
                        break;

                    case PERIODO_ANO_ATUAL:
                        dataIni = new DateTime(dataIni.Year, 1, 1);
                        sqlWhere.Append(" AND lancamentos.anoMes >= @anoMes");
                        banco.AddParameter("anoMes", dataIni.ToString("yyyyMM"));
                        break;

                    case Pesquisa.PERIODO_MANDATO_53:
                        sqlWhere.Append(" AND lancamentos.anoMes BETWEEN 200702 AND 201101");
                        break;

                    case Pesquisa.PERIODO_MANDATO_54:
                        sqlWhere.Append(" AND lancamentos.anoMes BETWEEN 201102 AND 201501");
                        break;

                    case Pesquisa.PERIODO_MANDATO_55:
                        sqlWhere.Append(" AND lancamentos.anoMes BETWEEN 201502 AND 201901");
                        break;

                    case PERIODO_ANO_ANTERIOR:
                        dataIni = new DateTime(dataIni.Year, 1, 1).AddYears(-1);
                        dataFim = new DateTime(dataIni.Year, 12, 31);
                        sqlWhere.Append(" AND lancamentos.anoMes BETWEEN @anoMesIni AND @anoMesFim");
                        banco.AddParameter("anoMesIni", dataIni.ToString("yyyyMM"));
                        banco.AddParameter("anoMesFim", dataFim.ToString("yyyyMM"));
                        break;

                    case PERIODO_INFORMAR:
                        dataIni = new DateTime(Convert.ToInt32(anoIni), Convert.ToInt32(mesIni), 1);
                        dataFim = new DateTime(Convert.ToInt32(anoFim), Convert.ToInt32(mesFim), 1);
                        sqlWhere.Append(" AND lancamentos.anoMes BETWEEN @anoMesIni AND @anoMesFim");
                        banco.AddParameter("anoMesIni", dataIni.ToString("yyyyMM"));
                        banco.AddParameter("anoMesFim", dataFim.ToString("yyyyMM"));
                        break;
                }

                Int32 numMes = 0;

                if (separarMes == true)
                {
                    numMes = ((dataFim.Year - dataIni.Year) * 12) + dataFim.Month - dataIni.Month + 1;
                    DateTime dataIniTemp = new DateTime(dataIni.Year, dataIni.Month, 1);

                    for (Int32 i = 0; i < numMes; i++)
                    {
                        sqlCampos.Append("SUM(CASE WHEN anoMes = @AnoMes" + i.ToString() + " THEN vlrLiquido ELSE 0 END) AS '" + dataIniTemp.ToString("MM-yyyy") + "',");
                        banco.AddParameter("AnoMes" + i.ToString(), dataIniTemp.ToString("yyyyMM"));
                        dataIniTemp = dataIniTemp.AddMonths(1);
                    }
                }

                sqlCampos.Append(" SUM(lancamentos.vlrLiquido) AS vlrTotal");


                if (!string.IsNullOrEmpty(lstParlamentar))
                {
                    sqlWhere.Append(" AND lancamentos.ideCadastro IN (" + lstParlamentar + ")");
                }

                if (!string.IsNullOrEmpty(lstDespesa))
                {
                    sqlWhere.Append(" AND lancamentos.numSubCota IN (" + lstDespesa + ")");
                }

                if (!string.IsNullOrEmpty(lstFornecedor))
                {
                    sqlWhere.Append(" AND lancamentos.txtCNPJCPF IN ('" + lstFornecedor.Replace(",", "','").Replace(".", "").Replace("-", "").Replace("/", "").Replace("'", "") + "')");
                }

                if (!string.IsNullOrEmpty(lstUF))
                {
                    sqlWhere.Append(" AND lancamentos.sgUF IN ('" + lstUF.Replace(",", "','") + "')");
                }


                if (!string.IsNullOrEmpty(lstPartido))
                {
                    sqlWhere.Append(" AND lancamentos.sgPartido IN ('" + lstPartido.Replace(",", "','") + "')");
                }

                if (documento != null && documento != "")
                {
                    String[] valores = documento.Split('|');
                    sqlWhere.Append(" AND lancamentos.txtNumero  = '" + valores[0].Trim() + "'");
                    sqlWhere.Append(" AND lancamentos.txtCNPJCPF = '" + valores[1].Trim() + "'");
                }

                sqlCmd.Append(sqlCampos.ToString());
                sqlCmd.Append(sqlFrom.ToString());
                sqlCmd.Append(sqlWhere.ToString());

                switch (agrupamento)
                {
                    case AGRUPAMENTO_PARLAMENTAR:
                        sqlCmd.Append(" GROUP BY 1, 2, 3, 4, 6");

                        if (separarMes == true)
                            sqlCmd.Append(" ORDER BY " + (numMes + 7).ToString() + " DESC");
                        else
                            sqlCmd.Append(" ORDER BY 7 DESC");

                        break;

                    case AGRUPAMENTO_DESPESA:
                    case AGRUPAMENTO_PARTIDO:
                    case AGRUPAMENTO_UF:
                        sqlCmd.Append(" GROUP BY 1, 2");

                        if (separarMes == true)
                            sqlCmd.Append(" ORDER BY " + (numMes + 3).ToString() + " DESC");
                        else
                            sqlCmd.Append(" ORDER BY 3 DESC");

                        break;

                    case AGRUPAMENTO_FORNECEDOR:
                        sqlCmd.Append(" GROUP BY 1, 2, 3, 4, 5, 6, 7");

                        if (separarMes == true)
                            sqlCmd.Append(" ORDER BY " + (numMes + 8).ToString() + " DESC");
                        else
                            sqlCmd.Append(" ORDER BY 8 DESC");

                        break;

                    case AGRUPAMENTO_DOCUMENTO:
                        sqlCmd.Append(" GROUP BY 1, 2, 3, 4, 5, 6, 7, 8, 9, 10");

                        if (separarMes == true)
                            sqlCmd.Append(" ORDER BY " + (numMes + 11).ToString() + " DESC");
                        else
                            sqlCmd.Append(" ORDER BY 11 DESC");

                        //sqlCmd.Append(" ORDER BY lancamentos.datEmissao");

                        break;
                }

                sqlCmd.Append(" LIMIT 1000");

                DbEstatisticas.InsereEstatisticaPesquisa("DEPFEDERAL", agrupamento, periodo, userName, sqlCmd.ToString(), anoIni, mesIni, anoFim, mesFim);

                using (MySqlDataReader reader = banco.ExecuteReader(sqlCmd.ToString(), 300))
                {
                    DataTable table = new DataTable("agrupamento");
                    table.Load(reader);

                    if (agrupamento == AGRUPAMENTO_DOCUMENTO)
                    {
                        Int64 recibo = 0;

                        foreach (DataRow row in table.Rows)
                        {
                            if (Int64.TryParse(row[1].ToString(), out recibo))
                                row[1] = recibo.ToString("000000000");
                        }
                    }


                    FormataColunas(agrupamento, table);

                    HttpContext.Current.Session["AuditoriaUltimaConsulta" + ChavePesquisa] = table;
                    grid.DataSource = table;
                    grid.DataBind();

                }
            }
        }

        private void FormataColunas(String agrupamento, DataTable table)
        {
            int i = 1;
            switch (agrupamento)
            {
                case AGRUPAMENTO_PARLAMENTAR:
                    table.Columns[i++].ColumnName = "Deputado(a)";
                    table.Columns[i++].ColumnName = "UF";
                    table.Columns[i++].ColumnName = "Partido";
                    table.Columns[i++].ColumnName = "Total NF/Recibos";
                    table.Columns[table.Columns.Count - 1].ColumnName = "Valor Total";
                    break;

                case AGRUPAMENTO_DESPESA:
                    table.Columns[1].ColumnName = "Despesa";
                    table.Columns[table.Columns.Count - 1].ColumnName = "Valor Total";
                    break;

                case AGRUPAMENTO_FORNECEDOR:
                    table.Columns[i++].ColumnName = "Razão Social";
                    table.Columns[i++].ColumnName = "CNPJ/CPF";
                    table.Columns[i++].ColumnName = "UF";

                    //Setar a INDEX_COLUNA_AUDITEI
                    table.Columns[i++].ColumnName = "Auditei?";

                    table.Columns[i++].ColumnName = "Última NF";
                    table.Columns[i++].ColumnName = "Doador?";
                    table.Columns[table.Columns.Count - 1].ColumnName = "Valor Total";
                    break;

                case AGRUPAMENTO_PARTIDO:
                    table.Columns[1].ColumnName = "Partido";
                    table.Columns[table.Columns.Count - 1].ColumnName = "Valor Total";
                    break;

                case AGRUPAMENTO_UF:
                    table.Columns[1].ColumnName = "UF";
                    table.Columns[table.Columns.Count - 1].ColumnName = "Valor Total";
                    break;

                case AGRUPAMENTO_DOCUMENTO:
                    table.Columns[i++].ColumnName = "NF/Recibo";
                    table.Columns[i++].ColumnName = "Data Emissão";
                    table.Columns[i++].ColumnName = "CNPJ/CPF";
                    table.Columns[i++].ColumnName = "Razão Social/Nome";
                    table.Columns[i++].ColumnName = "Deputado(a)";
                    table.Columns[table.Columns.Count - 1].ColumnName = "Valor Total";
                    break;
            }
        }

        public void DocumentosFornecedor(GridView grid, String cnpj, String codParlamentar)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("    SELECT DISTINCT");
            sql.Append("           lancamentos.txNomeParlamentar,");
            sql.Append("           lancamentos.txtNumero,");
            sql.Append("           lancamentos.datEmissao,");
            sql.Append("           lancamentos.vlrDocumento,");
            sql.Append("           parlamentares.nuDeputadoId,");
            sql.Append("           lancamentos.numano,");
            sql.Append("           lancamentos.ideDocumento");
            sql.Append("      FROM lancamentos");
            sql.Append(" LEFT JOIN parlamentares");
            sql.Append("        ON parlamentares.ideCadastro = lancamentos.ideCadastro");
            sql.Append("     WHERE lancamentos.txtCNPJCPF    = @txtCNPJCPF");
            sql.Append("       AND lancamentos.ideCadastro   = @ideCadastro");
            sql.Append("  ORDER BY 1, 3 DESC");
            sql.Append("     LIMIT 1000");

            using (Banco banco = new Banco())
            {
                banco.AddParameter("txtCNPJCPF", cnpj);
                banco.AddParameter("ideCadastro", codParlamentar);

                using (MySqlDataReader reader = banco.ExecuteReader(sql.ToString(), 300))
                {
                    DataTable table = new DataTable("lancamentos");
                    table.Load(reader);

                    Int64 recibo = 0;

                    foreach (DataRow row in table.Rows)
                    {
                        if (Int64.TryParse(row[1].ToString(), out recibo))
                            row[1] = recibo.ToString("000000000");
                    }

                    table.Columns[0].ColumnName = "Deputado Federal";
                    table.Columns[1].ColumnName = "NF/Recibo";
                    table.Columns[2].ColumnName = "Data Emissão";
                    table.Columns[3].ColumnName = "Valor Documento";

                    grid.DataSource = table;
                    grid.DataBind();
                }
            }
        }
    }
}