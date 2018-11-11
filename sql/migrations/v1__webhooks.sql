USE webhooks;

-- webhooks
CREATE TABLE subscriptions (
    subscription_id INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
    name            VARCHAR(250)       NOT NULL,
    uri             TEXT               NOT NULL
);

CREATE TABLE event_subscriptions (
    subscription_id INT      NOT NULL REFERENCES subscriptions (subscription_id),
    event_id        INT      NOT NULL REFERENCES events (event_id),
    created_at      DATETIME NOT NULL DEFAULT current_timestamp,
    published_at    DATETIME NULL,
    CONSTRAINT PRIMARY KEY (subscription_id, event_id)
);

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
    event_type_id   INT                NOT NULL REFERENCES event_types (event_type_id),
    body            TEXT               NULL,
    acknowledged_at DATETIME           NULL
);

