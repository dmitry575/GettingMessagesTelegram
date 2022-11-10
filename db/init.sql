CREATE TABLE IF NOT EXISTS public."Messages"
(
"Id" bigint NOT NULL,
"BaseId" bigint NOT NULL,
"Author" varchar(120) NOT NULL,
"Content" text,
"DateCreated" timestamp without time zone NOT NULL,
"ViewCount" int NOT NULL,
"CommentCount" int NOT NULL,
"ChannelId" bigint NOT NULL,
"GroupId" bigint NOT NULL
);

CREATE TABLE IF NOT EXISTS public."MessagesTranslates"
(
"Id" bigint NOT NULL,
"MessageId" bigint NOT NULL,
"Content" text,
"Language" varchar(5) NOT NULL,
"DateCreated" timestamp without time zone NOT NULL
);


CREATE TABLE IF NOT EXISTS public."Channels"
(
"Id" bigint NOT NULL,
"BaseId" bigint NOT NULL,
"Author" varchar(120) NOT NULL,
"Content" text,
"DateCreated" timestamp without time zone NOT NULL,
"HashAccess" bigint NOT NULL,
"MessagesCount" int NOT NULL
);

CREATE TABLE IF NOT EXISTS public."Comments"
(
"Id" bigint NOT NULL,
"BaseId" bigint NOT NULL,
"Author" varchar(120) NOT NULL,
"UserId" bigint,
"Content" text,
"DateCreated" timestamp without time zone NOT NULL,
"MessageId" bigint NOT NULL
);

CREATE TABLE IF NOT EXISTS public."CommentsTranslates"
(
"Id" bigint NOT NULL,
"CommentId" bigint NOT NULL,
"Language" varchar(5) NOT NULL,
"Content" text,
"DateCreated" timestamp without time zone NOT NULL
);

CREATE TABLE IF NOT EXISTS public."Medias"
(
"Id" bigint NOT NULL,
"BaseId" bigint NOT NULL,
"HashAccess" bigint NOT NULL,
"MessageId" bigint NOT NULL,
"Type" int NOT NULL,
"Url" varchar(512) NULL,
"Description" text NULL,
"MimeType" varchar(55) NULL,
"FileSize" bigint NULL,
"FileName" varchar(255) NULL,
"LocalPath" varchar(255) NULL,
"UrlExternal" varchar(255) NULL
);

CREATE SEQUENCE channels_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE channels_id_seq OWNED BY "Channels"."Id";

CREATE SEQUENCE messages_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE messages_id_seq OWNED BY "Messages"."Id";

CREATE SEQUENCE comments_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

CREATE SEQUENCE messages_translates_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER SEQUENCE messages_translates_id_seq OWNED BY "MessagesTranslates"."Id";

CREATE SEQUENCE comments_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER SEQUENCE comments_id_seq OWNED BY "Comments"."Id";

CREATE SEQUENCE comments_translates_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER SEQUENCE comments_translates_id_seq OWNED BY "Comments"."Id";

CREATE SEQUENCE medias_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER SEQUENCE medias_id_seq OWNED BY "Medias"."Id";

ALTER TABLE ONLY "Channels" ALTER COLUMN "Id" SET DEFAULT nextval('channels_id_seq'::regclass);

ALTER TABLE ONLY "Messages" ALTER COLUMN "Id" SET DEFAULT nextval('messages_id_seq'::regclass);

ALTER TABLE ONLY "MessagesTranslates" ALTER COLUMN "Id" SET DEFAULT nextval('messages_translates_id_seq'::regclass);

ALTER TABLE ONLY "Comments" ALTER COLUMN "Id" SET DEFAULT nextval('comments_id_seq'::regclass);

ALTER TABLE ONLY "CommentsTranslates" ALTER COLUMN "Id" SET DEFAULT nextval('comments_translates_id_seq'::regclass);

ALTER TABLE ONLY "Medias" ALTER COLUMN "Id" SET DEFAULT nextval('medias_id_seq'::regclass);


ALTER TABLE ONLY "Channels" ADD CONSTRAINT ix_channels_id PRIMARY KEY ("Id");

ALTER TABLE ONLY "Messages" ADD CONSTRAINT ix_messages_id PRIMARY KEY ("Id");

ALTER TABLE ONLY "MessagesTranslates" ADD CONSTRAINT ix_messages_translates_id PRIMARY KEY ("Id");

ALTER TABLE ONLY "Comments" ADD CONSTRAINT ix_comments_id PRIMARY KEY ("Id");

ALTER TABLE ONLY "CommentsTranslates" ADD CONSTRAINT ix_comments_translates_id PRIMARY KEY ("Id");

ALTER TABLE ONLY "Medias" ADD CONSTRAINT ix_medias_id PRIMARY KEY ("Id");


CREATE UNIQUE INDEX channels_baseid ON "Channels" USING btree ("BaseId");

CREATE UNIQUE INDEX messages_baseid ON "Messages" USING btree ("BaseId", "ChannelId");

CREATE INDEX messages_groupid ON "Messages" USING btree ("GroupId", "MessageId");

CREATE UNIQUE INDEX comments_baseid ON "Comments" USING btree ("BaseId", "MessageId");

CREATE UNIQUE INDEX medias_baseid ON "Medias" USING btree ("BaseId", "MessageId");

CREATE UNIQUE INDEX messages_translates_lang ON "MessagesTranslates" USING btree ("Language","MessageId");

CREATE UNIQUE INDEX comments_translates_lang ON "CommentsTranslates" USING btree ("Language","CommentId");

