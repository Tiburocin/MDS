create database SAEP;
use SAEP;

--Tablas, tablas intermedias y catalogos
create table c_Tipo_Evento(id_tipo integer identity(1,1) primary key, descripcion varchar(30) not null);
create table c_Estado_Evento(id_estado integer identity(1,1) primary key, descripcion varchar(30) not null);
create table c_Direccion(id_direccion integer identity(1,1) primary key, nombre varchar(50) not null, siglas varchar(10) not null);
create table c_Rol(id_rol integer identity(1,1) primary key, descripcion varchar(30) not null);
create table c_Programa_Edu(id_pro_edu integer identity(1,1) primary key, nombre varchar(40) not null, siglas varchar(10) not null, codigo_QR varchar(40), id_direccion integer foreign key references c_Direccion(id_direccion));
create table Usuario(matricula int primary key, nombre varchar(40) not null, correo varchar(40) not null, contraseña varchar(10) not null, id_rol integer foreign key references c_Rol(id_rol) not null, telefono varchar(15), id_pro_edu integer foreign key references c_Programa_Edu(id_pro_edu));
create table Evento(id_evento integer identity (1,1) primary key,codigo varchar(30) not null, matricula_co int foreign key references Usuario(matricula),id_estado integer foreign key references c_Estado_Evento(id_estado) not null, ponente varchar(40) not null, titulo varchar(30) not null, fecha date not null, lugar varchar(30) not null, asesor varchar(40) not null, id_tipo integer foreign key references c_Tipo_Evento(id_tipo) not null, abstract varchar(max) not null);


--Insersiones en las tablas catalogo
insert into c_Tipo_Evento(descripcion) values('Seminario');
insert into c_Tipo_Evento(descripcion) values('Coloquio');
insert into c_Tipo_Evento(descripcion) values('Examen de grado');

insert into c_Estado_Evento(descripcion) values('Espera');
insert into c_Estado_Evento(descripcion) values('Cancelado');
insert into c_Estado_Evento(descripcion) values('Realizado');


insert into c_Rol(descripcion) values('Director');
insert into c_Rol(descripcion) values('Coordinador');

insert into c_Direccion(nombre,siglas) values('Dirección de Investigación y Posgrado','DIP');

insert into c_Programa_Edu(nombre,siglas,codigo_QR,id_direccion) values('Maestria de Desarrollo de Software','MDS',null,1);

--Insersion de Usuarios
insert into Usuario(matricula,nombre,telefono,correo,contraseña,id_rol) values(823,'Juan Vargas','775-138-91-74','juan.vargas@upt.edu.mx',12345,1);

--Seleccion de tablas
select * from c_Rol;
select * from c_Direccion;
select * from c_Tipo_Evento;
select * from c_Estado_Evento;
select * from Usuario;
select * from c_Programa_Edu;