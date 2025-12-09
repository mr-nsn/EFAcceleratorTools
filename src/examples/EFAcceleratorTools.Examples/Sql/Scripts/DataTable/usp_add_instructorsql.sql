create procedure [dbo].[USP_ADD_INSTRUCTOR]
    @INSTRUCTORS [dbo].[TP_TB_INSTRUCTOR] readonly
as
begin
    set nocount on;

    insert into [dbo].[TB_INSTRUCTOR] ([TX_FULL_NAME], [DT_CREATION])
    select [TX_FULL_NAME], [DT_CREATION]
    from @INSTRUCTORS
end;