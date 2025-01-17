--
-- PostgreSQL database dump
--

-- Dumped from database version 17.2
-- Dumped by pg_dump version 17.2

-- Started on 2025-01-17 19:20:57

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 852 (class 1247 OID 16748)
-- Name: card_type; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.card_type AS ENUM (
    'SpellCard',
    'MonsterCard'
);


ALTER TYPE public.card_type OWNER TO postgres;

--
-- TOC entry 855 (class 1247 OID 16754)
-- Name: coin_type; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.coin_type AS ENUM (
    'Bronze',
    'Silver',
    'Gold',
    'Platinum',
    'Diamond'
);


ALTER TYPE public.coin_type OWNER TO postgres;

--
-- TOC entry 858 (class 1247 OID 16766)
-- Name: element_type; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.element_type AS ENUM (
    'Fire',
    'Water',
    'Normal'
);


ALTER TYPE public.element_type OWNER TO postgres;

--
-- TOC entry 861 (class 1247 OID 16774)
-- Name: monster_type; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.monster_type AS ENUM (
    'Dragon',
    'Goblin',
    'Knight',
    'Wizard',
    'Ork',
    'FireElve',
    'Kraken'
);


ALTER TYPE public.monster_type OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 218 (class 1259 OID 16793)
-- Name: cards; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cards (
    id integer NOT NULL,
    name character varying(255) NOT NULL,
    damage double precision NOT NULL,
    elementtype public.element_type NOT NULL,
    monstertype public.monster_type,
    card_type public.card_type NOT NULL
);


ALTER TABLE public.cards OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 16792)
-- Name: cards_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cards_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cards_id_seq OWNER TO postgres;

--
-- TOC entry 4900 (class 0 OID 0)
-- Dependencies: 217
-- Name: cards_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cards_id_seq OWNED BY public.cards.id;


--
-- TOC entry 221 (class 1259 OID 16817)
-- Name: coin_purses; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.coin_purses (
    user_id integer NOT NULL,
    bronze integer DEFAULT 0,
    silver integer DEFAULT 0,
    gold integer DEFAULT 0,
    platinum integer DEFAULT 0,
    diamond integer DEFAULT 0
);


ALTER TABLE public.coin_purses OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 16827)
-- Name: tokens; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tokens (
    user_id integer NOT NULL,
    token character varying(36) NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public.tokens OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 16838)
-- Name: user_cards; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.user_cards (
    user_id integer NOT NULL,
    card_id integer NOT NULL,
    in_deck boolean NOT NULL,
    card_type public.card_type NOT NULL,
    instance_id uuid DEFAULT gen_random_uuid() NOT NULL
);


ALTER TABLE public.user_cards OWNER TO postgres;

--
-- TOC entry 220 (class 1259 OID 16803)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    id integer NOT NULL,
    username character varying(255) NOT NULL,
    password character varying(255) NOT NULL,
    elo integer DEFAULT 100 NOT NULL,
    defeats integer DEFAULT 0 NOT NULL,
    wins integer DEFAULT 0 NOT NULL,
    draws integer DEFAULT 0 NOT NULL
);


ALTER TABLE public.users OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 16802)
-- Name: users_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.users_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.users_id_seq OWNER TO postgres;

--
-- TOC entry 4901 (class 0 OID 0)
-- Dependencies: 219
-- Name: users_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.users_id_seq OWNED BY public.users.id;


--
-- TOC entry 4724 (class 2604 OID 16796)
-- Name: cards id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cards ALTER COLUMN id SET DEFAULT nextval('public.cards_id_seq'::regclass);


--
-- TOC entry 4725 (class 2604 OID 16806)
-- Name: users id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN id SET DEFAULT nextval('public.users_id_seq'::regclass);


--
-- TOC entry 4738 (class 2606 OID 16798)
-- Name: cards cards_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cards
    ADD CONSTRAINT cards_pkey PRIMARY KEY (id);


--
-- TOC entry 4744 (class 2606 OID 16826)
-- Name: coin_purses coin_purses_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.coin_purses
    ADD CONSTRAINT coin_purses_pkey PRIMARY KEY (user_id);


--
-- TOC entry 4746 (class 2606 OID 16832)
-- Name: tokens tokens_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tokens
    ADD CONSTRAINT tokens_pkey PRIMARY KEY (token);


--
-- TOC entry 4748 (class 2606 OID 16843)
-- Name: user_cards user_cards_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_cards
    ADD CONSTRAINT user_cards_pkey PRIMARY KEY (user_id, card_id, instance_id);


--
-- TOC entry 4740 (class 2606 OID 16814)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- TOC entry 4742 (class 2606 OID 16816)
-- Name: users users_username_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_username_key UNIQUE (username);


--
-- TOC entry 4749 (class 2606 OID 16833)
-- Name: tokens tokens_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tokens
    ADD CONSTRAINT tokens_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;


-- Completed on 2025-01-17 19:20:57

--
-- PostgreSQL database dump complete
--

