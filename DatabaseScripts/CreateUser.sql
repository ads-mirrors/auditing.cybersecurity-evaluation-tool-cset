--this is an example script 
--please rename INEL-NT\WEB_SQL_CSET_ACC to your user name
IF NOT EXISTS (SELECT * FROM master.dbo.syslogins WHERE loginname = N'INEL-NT\WEB_SQL_CSET_ACC')
CREATE LOGIN [INEL-NT\WEB_SQL_CSET_ACC] FROM WINDOWS
GO
CREATE USER [INEL-NT\web_sql_cset_acc] FOR LOGIN [INEL-NT\WEB_SQL_CSET_ACC]
GO
GRANT EXECUTE ON SCHEMA:: [dbo] TO [INEL-NT\web_sql_cset_acc]
