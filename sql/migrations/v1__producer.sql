USE producer;

-- ordering system
-- this could be any business system that raises some kind of event which should be sent to subscribers

CREATE TABLE orders (
    order_id     INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
    product_code VARCHAR(50)        NOT NULL,
    created_at   DATETIME           NOT NULL DEFAULT current_timestamp,
    processed_at DATETIME           NULL
);

CREATE TABLE event_types (
    event_type_id INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
    code          VARCHAR(50)        NOT NULL UNIQUE
);

INSERT INTO event_types (event_type_id, code)
VALUES (1, 'OrderCreated');
INSERT INTO event_types (event_type_id, code)
VALUES (2, 'OrderProcessed');

CREATE TABLE events (
    event_id        INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
    event_type_id   INT                NOT NULL,
    body            TEXT               NULL,
    acknowledged_at DATETIME           NULL,
    FOREIGN KEY (event_type_id) REFERENCES event_types (event_type_id)
);

-- webhooks
CREATE TABLE subscriptions (
    subscription_id INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
    name            VARCHAR(250)       NOT NULL,
    uri             TEXT               NOT NULL
);

CREATE TABLE event_subscriptions (
    subscription_id INT      NOT NULL,
    event_id        INT      NOT NULL,
    created_at      DATETIME NOT NULL DEFAULT current_timestamp,
    published_at    DATETIME NULL,
    PRIMARY KEY (subscription_id, event_id),
    FOREIGN KEY (subscription_id) REFERENCES subscriptions (subscription_id),
    FOREIGN KEY (event_id) REFERENCES events (event_id)
);


/*

INSERT INTO events (event_type_id, body) VALUES (1, '{"OrderId": 109}');
INSERT INTO event_subscriptions (event_id, subscription_id) VALUES (LAST_INSERT_ID(), 1);

SELECT * FROM events;
SELECT * FROM orders;

SELECT * FROM event_subscriptions;

DELETE FROM event_subscriptions;
DELETE FROM events;
DELETE FROM orders;

*/
