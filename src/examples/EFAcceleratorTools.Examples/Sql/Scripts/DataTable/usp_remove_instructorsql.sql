create procedure [dbo].[USP_REMOVE_INSTRUCTOR]
    @FULLNAME [nvarchar](100)
as
begin
    set nocount on;

    delete from [dbo].[TB_INSTRUCTOR]
    where [TX_FULL_NAME] = @FULLNAME
end;