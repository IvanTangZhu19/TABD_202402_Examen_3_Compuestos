-- docker run --name postgres-Compuestos -e POSTGRES_PASSWORD=unaClav3 -d -p 5432:5432 postgres:latest

-- Para entrar desde el contenedor con usuario root (postgres)
psql -U postgres

create database compuestos_quimicos_db;

create user compuestos_app with encrypted password 'unaClav3';

create schema core;

grant connect on database compuestos_quimicos_db to compuestos_app;
grant create on database compuestos_quimicos_db to compuestos_app;
grant create, usage on schema core to compuestos_app;
alter user compuestos_app set search_path to core;

-- usuario para conexión
create user compuestos_usr with encrypted password 'unaClav3';

grant connect on database compuestos_quimicos_db to compuestos_usr;
grant usage on schema core to compuestos_usr;
alter default privileges for user compuestos_app in schema core grant insert, update, delete, select on tables to compuestos_usr;
alter default privileges for user compuestos_app in schema core grant execute on routines TO compuestos_usr;
alter user compuestos_usr set search_path to core;

create extension if not exists "uuid-ossp";

---------------------------
-- Creación de tablas
--------------------------

create table core.elementos
(
    id      	    integer generated always as identity constraint elementos_pk primary key,
    nombre  	    varchar(50) not null,
    simbolo         varchar(4) not null,
    numero_atomico  integer not null,
    configuracion   varchar (150) not null,
    elemento_uuid   uuid default gen_random_uuid(),
    constraint elementos_nombre_uk unique (nombre)
);

create table core.compuestos
(
    id      	      integer generated always as identity constraint compuestos_pk primary key,
    nombre  	      varchar(50) not null,
    formula           varchar(20) not null,
    masa_molar        float not null,
    estado_agregacion varchar (20) not null,
    compuesto_uuid    uuid default gen_random_uuid(),
    constraint compuesto_nombre_uk unique (nombre)
);

create table core.elementos_compuesto
(
    compuestoID     integer not null constraint elementoCompuesto_compuesto_fk references core.compuestos,
    elementoID      integer not null constraint elementoCompuesto_elemento_fk references core.elementos,
    cantidad        integer not null,
    constraint elementosCompuesto_pk primary key (compuestoID, elementoID)
);


