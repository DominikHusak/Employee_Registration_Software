Create table UserAccount
(
id int primary key identity(1, 1),
jmeno varchar(20) not null,
prijmeni varchar(20) not null,
datum_narozeni date not null,
plat int not null,
oddeleni varchar(30)not null,
user_role varchar(10) not null,
heslo varchar(20) not null
);

CREATE TABLE Smena
(
id int primary key identity(1, 1)
user_id int not null,
cislo_skladu int not null,
den_smeny date not null,
cas_smeny time not null,
foreign key (user_id) references UserAccount(id)
);

