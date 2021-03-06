﻿using System;
using System.Web.UI.WebControls;
using SiteServer.BackgroundPages.Core;
using SiteServer.BackgroundPages.Utils;
using SiteServer.Utils;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.Core.RestRoutes;
using SiteServer.CMS.Core.RestRoutes.Sys.Stl;
using SiteServer.CMS.Fx;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
	public class ModalUserExport : BasePage
    {
        public PlaceHolder PhExport;
        public DropDownList DdlCheckedState;
        public Button BtnSubmit;

        public static string GetOpenWindowString()
        {
            return LayerUtils.GetOpenScript("导出用户", FxUtils.GetSettingsUrl(nameof(ModalUserExport), null), 450, 270);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (!IsPostBack)
			{
                PhExport.Visible = true;
                FxUtils.AddListItemsToETriState(DdlCheckedState, "全部", "审核通过", "未审核");
			    SystemWebUtils.SelectSingleItem(DdlCheckedState, ETriStateUtils.GetValue(ETriState.All));
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                PhExport.Visible = false;

                const string fileName = "users.csv";
                var filePath = PathUtils.GetTemporaryFilesPath(fileName);

                ExcelObject.CreateExcelFileForUsers(filePath, ETriStateUtils.GetEnumType(DdlCheckedState.SelectedValue));

                var link = new HyperLink
                {
                    NavigateUrl = ApiRouteActionsDownload.GetUrl(ApiManager.InnerApiUrl, filePath),
                    Text = "下载"
                };
                var successMessage = "成功导出文件！&nbsp;&nbsp;" + ControlUtils.GetControlRenderHtml(link);
                SuccessMessage(successMessage);

                BtnSubmit.Visible = false;
            }
            catch (Exception ex)
            {
                var failedMessage = "文件导出失败！<br/><br/>原因为：" + ex.Message;
                FailMessage(ex, failedMessage);
            }
		}
	}
}
