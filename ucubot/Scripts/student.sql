create table student (
  Id int not null auto_increment,
  FirstName varchar(128) not null, 
  LastName varchar(128) not null, 
  UserId varchar(128) not null unique, 
  primary key (id));
