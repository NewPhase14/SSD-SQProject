CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
SET TIME ZONE 'CET';

CREATE TABLE users (
                       id TEXT PRIMARY KEY,
                       name TEXT NOT NULL,
                       email TEXT NOT NULL,
                       password_hash TEXT NOT NULL,
                       password_salt TEXT NOT NULL,
                       created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                       updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                       tfa_secret BYTEA,
                       nonce BYTEA,
                       tag BYTEA,
                       is_tfa_enabled BOOLEAN NOT NULL
);

CREATE TABLE categories (
                        id TEXT PRIMARY KEY DEFAULT uuid_generate_v4(),
                        name TEXT NOT NULL
);

CREATE TABLE listings (
                          id TEXT PRIMARY KEY,
                          user_id TEXT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
                          category_id TEXT NOT NULL REFERENCES categories(id),
                          condition VARCHAR(30) NOT NULL,
                          title VARCHAR(255) NOT NULL,
                          description TEXT NOT NULL,
                          price DECIMAL(10,2) NOT NULL,
                          status VARCHAR(30) NOT NULL,
                          created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                          updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);


CREATE TABLE images (
                        id TEXT PRIMARY KEY,
                        image_url TEXT NOT NULL,
                        public_id TEXT NOT NULL,
                        listing_id TEXT NOT NULL REFERENCES listings(id) ON DELETE CASCADE,
                        created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE conversations (
                               id TEXT PRIMARY KEY,
                               listing_id TEXT NOT NULL REFERENCES listings(id) ON DELETE CASCADE,
                               buyer_user_id TEXT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
                               created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                               UNIQUE(listing_id, buyer_user_id)
);

CREATE TABLE messages (
                          id TEXT PRIMARY KEY,
                          conversation_id TEXT NOT NULL REFERENCES conversations(id) ON DELETE CASCADE,
                          sender_user_id TEXT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
                          ciphertext BYTEA NOT NULL,
                          nonce BYTEA NOT NULL,
                          tag BYTEA NOT NULL,
                          created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

INSERT INTO categories (name) VALUES
                                  ('Cars'),
                                  ('Motorcycles'),
                                  ('Scooters'),
                                  ('Bicycles'),
                                  ('Electronics'),
                                  ('Mobile Phones'),
                                  ('Computers'),
                                  ('Tablets'),
                                  ('TV & Audio'),
                                  ('Cameras'),
                                  ('Gaming'),
                                  ('Game Consoles'),
                                  ('Smartwatches'),
                                  ('Home'),
                                  ('Furniture'),
                                  ('Lighting'),
                                  ('Kitchen Equipment'),
                                  ('Clothing'),
                                  ('Men Clothing'),
                                  ('Women Clothing'),
                                  ('Children Clothing'),
                                  ('Shoes'),
                                  ('Bags'),
                                  ('Jewelry'),
                                  ('Accessories'),
                                  ('Sports'),
                                  ('Fitness'),
                                  ('Outdoor'),
                                  ('Camping'),
                                  ('Books'),
                                  ('Music'),
                                  ('Movies'),
                                  ('Toys'),
                                  ('Garden'),
                                  ('Tools'),
                                  ('Pets'),
                                  ('Health'),
                                  ('Beauty'),
                                  ('Office Equipment'),
                                  ('Musical Instruments'),
                                  ('Other');

CREATE INDEX idx_conversations_listing_buyer
    ON conversations (listing_id, buyer_user_id);

CREATE INDEX idx_messages_conversation_created
    ON messages (conversation_id, created_at);