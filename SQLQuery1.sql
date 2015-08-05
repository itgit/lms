-- use [aspnet-MVCrepetition-20150727093829]
/*
select * from AspNetUsers
select * from AspNetRoles
select * from AspNetUserRoles
*/

select 
AspNetUsers.FirstName
from AspNetUserRoles
join AspNetUsers on AspNetUserRoles.UserId = AspNetUsers.Id
join AspNetRoles on AspNetUserRoles.RoleId = AspNetRoles.Id
where 
1 = 1
and AspNetRoles.Name like 'adm%'

select 
--email
--,username
*
from AspNetUsers
where UserName = 'yxkalle'

--update AspNetUsers
--set lastname='Bengtsson'
set firstname=NULL
where UserName = 'yxkalle'

