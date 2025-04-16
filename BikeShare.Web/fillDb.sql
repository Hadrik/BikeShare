BEGIN TRANSACTION;

-- Create tables
create table if not exists Roles
(
    role_id     integer not null
        constraint Roles_pk
            primary key autoincrement,
    name        text    not null,
    permissions integer not null
);

create table if not exists Stations
(
    station_id    integer not null
        constraint Stations_pk
            primary key autoincrement,
    name          text    not null,
    location_lat  real    not null,
    location_long real    not null
);

create table if not exists Bikes
(
    bike_id            integer not null
        constraint Bikes_pk
            primary key autoincrement,
    station_id         integer not null
        constraint Bikes_Stations_station_id_fk
            references Stations,
    status             text    not null,
    last_status_change integer not null
);

create table if not exists StatusHistory
(
    status_history_id integer not null
        constraint StatusHistory_pk
            primary key autoincrement,
    bike_id           integer not null
        constraint StatusHistory_Bikes_bike_id_fk
            references Bikes,
    station_id        integer
        constraint StatusHistory_Stations_station_id_fk
            references Stations,
    status            text    not null,
    timestamp         integer not null
);

create table if not exists Users
(
    user_id       integer not null
        constraint Users_pk
            primary key autoincrement,
    role_id       integer not null
        constraint Users_Roles_role_id_fk
            references Roles,
    email         text    not null
        constraint Users_email_unique
            unique,
    password_hash text    not null,
    username      text    not null
        constraint Users_username_unique
            unique
);

create table if not exists Rentals
(
    rental_id        integer not null
        constraint Rentals_pk
            primary key autoincrement,
    bike_id          integer not null
        constraint Rentals_Bikes_bike_id_fk
            references Bikes,
    user_id          integer not null
        constraint Rentals_Users_user_id_fk
            references Users,
    start_station_id integer not null
        constraint Rentals_Stations_station_id_fk
            references Stations,
    end_station_id   integer
        constraint Rentals_Stations_station_id_fk_2
            references Stations,
    start_timestamp  integer not null,
    end_timestamp    integer,
    distance         integer,
    cost             integer
);


-- Insert stations
INSERT INTO Stations(station_id, name, location_lat, location_long) VALUES (1, 'Downtown Hub', 18.161756698600577, 49.83191445274622);

INSERT INTO Stations(station_id, name, location_lat, location_long) VALUES (2, 'Riverside Station', 18.164243380651907, 49.83355878787478);

INSERT INTO Stations(station_id, name, location_lat, location_long) VALUES (3, 'University Dock', 18.15784904716084, 49.83622505389712);

-- Insert bikes
INSERT INTO Bikes(bike_id, station_id, status, last_status_change) VALUES (1, 1, 'Available', 1744808542);

INSERT INTO Bikes(bike_id, station_id, status, last_status_change) VALUES (2, 1, 'Available', 1744705042);

INSERT INTO Bikes(bike_id, station_id, status, last_status_change) VALUES (3, 2, 'Maintenance', 1744708361);

INSERT INTO Bikes(bike_id, station_id, status, last_status_change) VALUES (4, 2, 'Available', 1744564520);

INSERT INTO Bikes(bike_id, station_id, status, last_status_change) VALUES (5, 3, 'InUse', 1743354920);

INSERT INTO Bikes(bike_id, station_id, status, last_status_change) VALUES (6, 3, 'Available', 1743541641);

INSERT INTO Bikes(bike_id, station_id, status, last_status_change) VALUES (7, 3, 'Available', 1743714441);

-- Insert roles
INSERT INTO Roles(role_id, name, permissions) VALUES (1, 'Admin', 1);

INSERT INTO Roles(role_id, name, permissions) VALUES (2, 'User', 0);

-- Insert users
-- Password: adminadmin
INSERT INTO Users(user_id, role_id, email, password_hash, username) VALUES (1, 1, 'admin@admin.admin', '$2a$11$bKKX4dxvz0SufVftvpU5SO9/N1MjQXwCg83ZWLx7Dxl2mIIvU.ZrC', 'admin');

-- Password: useruser
INSERT INTO Users(user_id, role_id, email, password_hash, username) VALUES (2, 2, 'user@user.user', '$2a$11$nnVaMi9v3e/oGPy1X4FiIec8gABYzT/cqLRcX06mElvCi1Im6eBO2', 'user');

COMMIT;