﻿using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class ExportSettingModel : BaseModel
    {
        public string ReferenceType = "Sites";
        public long ReferenceId = 0;
        public Title Title = new Title();
        public long ExportSettingId = 0;
        public bool AddHeader = true;
        public ExportColumns ExportColumns = new ExportColumns();
        [NonSerialized] public string SavedReferenceType = "Sites";
        [NonSerialized] public long SavedReferenceId = 0;
        [NonSerialized] public string SavedTitle = string.Empty;
        [NonSerialized] public long SavedExportSettingId = 0;
        [NonSerialized] public bool SavedAddHeader = true;
        [NonSerialized] public string SavedExportColumns = string.Empty;

        public bool ReferenceType_Updated()
        {
            return ReferenceType != SavedReferenceType && ReferenceType != null;
        }

        public bool ReferenceId_Updated()
        {
            return ReferenceId != SavedReferenceId;
        }

        public bool Title_Updated()
        {
            return Title.Value != SavedTitle && Title.Value != null;
        }

        public bool ExportSettingId_Updated()
        {
            return ExportSettingId != SavedExportSettingId;
        }

        public bool AddHeader_Updated()
        {
            return AddHeader != SavedAddHeader;
        }

        public bool ExportColumns_Updated()
        {
            return ExportColumns.ToJson() != SavedExportColumns && ExportColumns.ToJson() != null;
        }

        public Title Session_Title()
        {
            return this.PageSession("Title") != null
                ? this.PageSession("Title") as Title
                : Title;
        }

        public void  Session_Title(object value)
        {
            this.PageSession("Title", value);
        }

        public bool Session_AddHeader()
        {
            return this.PageSession("AddHeader") != null
                ? this.PageSession("AddHeader").ToBool()
                : AddHeader;
        }

        public void  Session_AddHeader(object value)
        {
            this.PageSession("AddHeader", value);
        }

        public ExportColumns Session_ExportColumns()
        {
            return this.PageSession("ExportColumns") != null
                ? this.PageSession("ExportColumns")?.ToString().Deserialize<ExportColumns>() ?? new ExportColumns(ReferenceType)
                : ExportColumns;
        }

        public void  Session_ExportColumns(object value)
        {
            this.PageSession("ExportColumns", value);
        }

        public ExportSettingModel()
        {
        }

        public ExportSettingModel(
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public ExportSettingModel(
            long exportSettingId,
            bool clearSessions = false,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            ExportSettingId = exportSettingId;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public ExportSettingModel(DataRow dataRow, string tableAlias = null)
        {
            OnConstructing();
            Set(dataRow, tableAlias);
            OnConstructed();
        }

        private void OnConstructing()
        {
        }

        private void OnConstructed()
        {
        }

        public void ClearSessions()
        {
            Session_Title(null);
            Session_AddHeader(null);
            Session_ExportColumns(null);
        }

        public ExportSettingModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectExportSettings(
                tableType: tableType,
                column: column ?? Rds.ExportSettingsDefaultColumns(),
                join: join ??  Rds.ExportSettingsJoinDefault(),
                where: where ?? Rds.ExportSettingsWhereDefault(this),
                orderBy: orderBy,
                param: param,
                distinct: distinct,
                top: top)));
            return this;
        }

        public Error.Types Create(
            RdsUser rdsUser = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool paramAll = false,
            bool get = true)
        {
            var statements = new List<SqlStatement>();
            CreateStatements(statements, tableType, param, paramAll);
            var newId = Rds.ExecuteScalar_long(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            ExportSettingId = newId != 0 ? newId : ExportSettingId;
            if (get) Get();
            return Error.Types.None;
        }

        public List<SqlStatement> CreateStatements(
            List<SqlStatement> statements,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool paramAll = false)
        {
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertExportSettings(
                    tableType: tableType,
                        selectIdentity: true,
                    param: param ?? Rds.ExportSettingsParamDefault(
                        this, setDefault: true, paramAll: paramAll))
            });
            return statements;
        }

        public Error.Types Update(
            RdsUser rdsUser = null,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool paramAll = false,
            bool get = true)
        {
            SetBySession();
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            UpdateStatements(statements, timestamp, param, paramAll, additionalStatements);
            var count = Rds.ExecuteScalar_int(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            if (count == 0) return Error.Types.UpdateConflicts;
            if (get) Get();
            return Error.Types.None;
        }

        private List<SqlStatement> UpdateStatements(
            List<SqlStatement> statements,
            DateTime timestamp,
            SqlParamCollection param,
            bool paramAll = false,
            List<SqlStatement> additionalStatements = null)
        {
            statements.AddRange(new List<SqlStatement>
            {
                Rds.UpdateExportSettings(
                    verUp: VerUp,
                    where: Rds.ExportSettingsWhereDefault(this)
                        .UpdatedTime(timestamp, _using: timestamp.InRange()),
                    param: param ?? Rds.ExportSettingsParamDefault(this, paramAll: paramAll),
                    countRecord: true)
            });
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        public Error.Types UpdateOrCreate(
            RdsUser rdsUser = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession();
            var statements = new List<SqlStatement>
            {
                Rds.UpdateOrInsertExportSettings(
                    selectIdentity: true,
                    where: where ?? Rds.ExportSettingsWhereDefault(this),
                    param: param ?? Rds.ExportSettingsParamDefault(this, setDefault: true))
            };
            var newId = Rds.ExecuteScalar_long(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            ExportSettingId = newId != 0 ? newId : ExportSettingId;
            Get();
            return Error.Types.None;
        }

        public Error.Types Delete()
        {
            var statements = new List<SqlStatement>();
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteExportSettings(
                    where: Rds.ExportSettingsWhere().ExportSettingId(ExportSettingId))
            });
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: statements.ToArray());
            return Error.Types.None;
        }

        public Error.Types Restore(long exportSettingId)
        {
            ExportSettingId = exportSettingId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreExportSettings(
                        where: Rds.ExportSettingsWhere().ExportSettingId(ExportSettingId))
                });
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteExportSettings(
                    tableType: tableType,
                    param: Rds.ExportSettingsParam().ExportSettingId(ExportSettingId)));
            return Error.Types.None;
        }

        public void SetByForm()
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "ExportSettings_ReferenceType": ReferenceType = Forms.Data(controlId).ToString(); break;
                    case "ExportSettings_ReferenceId": ReferenceId = Forms.Data(controlId).ToLong(); break;
                    case "ExportSettings_Title": Title = new Title(ExportSettingId, Forms.Data(controlId)); break;
                    case "ExportSettings_AddHeader": AddHeader = Forms.Data(controlId).ToBool(); break;
                    case "ExportSettings_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
                    case "Comments": Comments.Prepend(Forms.Data("Comments")); break;
                    case "VerUp": VerUp = Forms.Data(controlId).ToBool(); break;
                    default:
                        if (controlId.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                controlId.Substring("Comment".Length).ToInt(),
                                Forms.Data(controlId));
                        }
                        break;
                }
            });
            if (Routes.Action() == "deletecomment")
            {
                DeleteCommentId = Forms.ControlId().Split(',')._2nd().ToInt();
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
            Forms.FileKeys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    default: break;
                }
            });
        }

        private void SetBySession()
        {
            if (!Forms.HasData("ExportSettings_Title")) Title = Session_Title();
            if (!Forms.HasData("ExportSettings_AddHeader")) AddHeader = Session_AddHeader();
            if (!Forms.HasData("ExportSettings_ExportColumns")) ExportColumns = Session_ExportColumns();
        }

        private void Set(DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(DataRow dataRow, string tableAlias = null)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
            {
                var column = new ColumnNameInfo(dataColumn.ColumnName);
                if (column.TableAlias == tableAlias)
                {
                    switch (column.Name)
                    {
                        case "ReferenceType":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                ReferenceType = dataRow[column.ColumnName].ToString();
                                SavedReferenceType = ReferenceType;
                            }
                            break;
                        case "ReferenceId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                ReferenceId = dataRow[column.ColumnName].ToLong();
                                SavedReferenceId = ReferenceId;
                            }
                            break;
                        case "Title":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                Title = new Title(dataRow, "ExportSettingId");
                                SavedTitle = Title.Value;
                            }
                            break;
                        case "ExportSettingId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                ExportSettingId = dataRow[column.ColumnName].ToLong();
                                SavedExportSettingId = ExportSettingId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "AddHeader":
                            AddHeader = dataRow[column.ColumnName].ToBool();
                            SavedAddHeader = AddHeader;
                            break;
                        case "ExportColumns":
                            ExportColumns = dataRow[column.ColumnName].ToString().Deserialize<ExportColumns>() ?? new ExportColumns(ReferenceType);
                            SavedExportColumns = ExportColumns.ToJson();
                            break;
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            SavedComments = Comments.ToJson();
                            break;
                        case "Creator":
                            Creator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "UpdatedTime":
                            UpdatedTime = new Time(dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
                            SavedUpdatedTime = UpdatedTime.Value;
                            break;
                        case "IsHistory": VerType = dataRow[column.ColumnName].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                    }
                }
            }
        }

        public bool Updated()
        {
            return
                ReferenceType_Updated() ||
                ReferenceId_Updated() ||
                Title_Updated() ||
                ExportSettingId_Updated() ||
                Ver_Updated() ||
                AddHeader_Updated() ||
                ExportColumns_Updated() ||
                Comments_Updated() ||
                Creator_Updated() ||
                Updator_Updated() ||
                CreatedTime_Updated() ||
                UpdatedTime_Updated();
        }
    }
}
