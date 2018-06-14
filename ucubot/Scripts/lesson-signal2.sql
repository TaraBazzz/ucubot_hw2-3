alter table lesson_signal drop UserId;
alter table lesson_signal add student_id int not null;
alter table lesson_signal add foreign key (student_id) references student(Id) on delete restrict on update restrict;