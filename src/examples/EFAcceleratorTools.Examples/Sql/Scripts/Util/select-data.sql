select
	*
from
	TB_COURSE a
inner join
	TB_MODULE c on a.SQ_COURSE = c.SQ_MODULE
inner join
	TB_LESSON d on c.SQ_MODULE = d.SQ_MODULE
inner join
	TB_INSTRUCTOR b on a.SQ_INSTRUCTOR = b.SQ_INSTRUCTOR
inner join
	TB_PROFILE e on b.SQ_INSTRUCTOR = e.SQ_INSTRUCTOR;

select * from TB_COURSE;
select * from TB_MODULE;
select * from TB_LESSON;
select * from TB_INSTRUCTOR;
select * from TB_PROFILE;