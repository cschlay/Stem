CREATE EXTENSION "uuid-ossp";

CREATE TABLE account (
    id SERIAL PRIMARY KEY,
    pid uuid DEFAULT uuid_generate_v4() UNIQUE NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL
);

INSERT INTO account (email) VALUES ('asd');
