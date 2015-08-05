select * from AspNetUsers;
select * from AspNetRoles;
select * from AspNetUserRoles;

select *
from AspNetUsers
Left outer join AspNetUserRoles on AspNetUserRoles.UserId = AspNetUsers.Id

select
 AspNetUserRoles.*, 
 AspNetUsers.*
from C --on AspNetUserRoles.UserId = AspNetUsers.Id

select * from  AspNetUserRoles, AspNetUsers
where AspNetUserRoles.UserId = AspNetUsers.Id

select UserId as 'idrubrik' from  AspNetUserRoles
union
select Id from  AspNetUsers;









