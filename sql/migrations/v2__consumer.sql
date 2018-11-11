USE consumer;

CREATE TABLE emails (
    email_id INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
    body TEXT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT current_timestamp,
    sent_at DATETIME NULL
);


/*

select * from emails;

*/