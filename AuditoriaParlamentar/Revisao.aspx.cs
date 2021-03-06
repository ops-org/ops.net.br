﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AuditoriaParlamentar
{
    public partial class Revisao : System.Web.UI.Page
    {
        protected override void OnPreInit(EventArgs e)
        {
            if (Session["MasterPage"] == "Farejador")
            {
                Page.MasterPageFile = "~/OpsFarejador.Master";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!System.Web.HttpContext.Current.User.Identity.IsAuthenticated || !System.Web.HttpContext.Current.User.IsInRole("REVISOR"))
                Response.Redirect("~/Account/Login.aspx?ReturnUrl=/Revisao.aspx");
        }
    }
}