PGDMP  /                    {            postgres    16.0    16.0 1    �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            �           1262    5    postgres    DATABASE     �   CREATE DATABASE postgres WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_United States.1252';
    DROP DATABASE postgres;
                postgres    false            �           0    0    DATABASE postgres    COMMENT     N   COMMENT ON DATABASE postgres IS 'default administrative connection database';
                   postgres    false    4843                        3079    16384 	   adminpack 	   EXTENSION     A   CREATE EXTENSION IF NOT EXISTS adminpack WITH SCHEMA pg_catalog;
    DROP EXTENSION adminpack;
                   false            �           0    0    EXTENSION adminpack    COMMENT     M   COMMENT ON EXTENSION adminpack IS 'administrative functions for PostgreSQL';
                        false    2            �            1259    16665    emaillabels    TABLE     P   CREATE TABLE public.emaillabels (
    email_id integer,
    label_id integer
);
    DROP TABLE public.emaillabels;
       public         heap    postgres    false            �            1259    16574    emails    TABLE     )  CREATE TABLE public.emails (
    email_id integer NOT NULL,
    project_id integer,
    sender_address character varying(100),
    receiver_address character varying(100),
    subject character varying(100),
    date_sent timestamp without time zone,
    email_content text,
    file_path text
);
    DROP TABLE public.emails;
       public         heap    postgres    false            �            1259    16573    emails_email_id_seq    SEQUENCE     �   CREATE SEQUENCE public.emails_email_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 *   DROP SEQUENCE public.emails_email_id_seq;
       public          postgres    false    222            �           0    0    emails_email_id_seq    SEQUENCE OWNED BY     K   ALTER SEQUENCE public.emails_email_id_seq OWNED BY public.emails.email_id;
          public          postgres    false    221            �            1259    24590    emails_email_id_seq1    SEQUENCE     �   ALTER TABLE public.emails ALTER COLUMN email_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.emails_email_id_seq1
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    222            �            1259    16588    labels    TABLE     }   CREATE TABLE public.labels (
    label_id integer NOT NULL,
    label_name character varying(100),
    created_by integer
);
    DROP TABLE public.labels;
       public         heap    postgres    false            �            1259    16587    labels_label_id_seq    SEQUENCE     �   CREATE SEQUENCE public.labels_label_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 *   DROP SEQUENCE public.labels_label_id_seq;
       public          postgres    false    224            �           0    0    labels_label_id_seq    SEQUENCE OWNED BY     K   ALTER SEQUENCE public.labels_label_id_seq OWNED BY public.labels.label_id;
          public          postgres    false    223            �            1259    16527    projects    TABLE     ~  CREATE TABLE public.projects (
    project_id integer NOT NULL,
    project_name character varying(100) NOT NULL,
    description text,
    start_date date,
    end_date date,
    permission_tag character varying(50),
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    owner_id integer
);
    DROP TABLE public.projects;
       public         heap    postgres    false            �            1259    16526    project_project_id_seq    SEQUENCE     �   CREATE SEQUENCE public.project_project_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 -   DROP SEQUENCE public.project_project_id_seq;
       public          postgres    false    219            �           0    0    project_project_id_seq    SEQUENCE OWNED BY     R   ALTER SEQUENCE public.project_project_id_seq OWNED BY public.projects.project_id;
          public          postgres    false    218            �            1259    16555    projectmembers    TABLE     v   CREATE TABLE public.projectmembers (
    project_id integer,
    user_id integer,
    status character varying(10)
);
 "   DROP TABLE public.projectmembers;
       public         heap    postgres    false            �            1259    16399    users    TABLE     (  CREATE TABLE public.users (
    user_id integer NOT NULL,
    username character varying(50) NOT NULL,
    email character varying(100) NOT NULL,
    password character varying(100) NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    role character varying(50)
);
    DROP TABLE public.users;
       public         heap    postgres    false            �            1259    16398    user_user_id_seq    SEQUENCE     �   CREATE SEQUENCE public.user_user_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 '   DROP SEQUENCE public.user_user_id_seq;
       public          postgres    false    217            �           0    0    user_user_id_seq    SEQUENCE OWNED BY     F   ALTER SEQUENCE public.user_user_id_seq OWNED BY public.users.user_id;
          public          postgres    false    216            8           2604    16591    labels label_id    DEFAULT     r   ALTER TABLE ONLY public.labels ALTER COLUMN label_id SET DEFAULT nextval('public.labels_label_id_seq'::regclass);
 >   ALTER TABLE public.labels ALTER COLUMN label_id DROP DEFAULT;
       public          postgres    false    224    223    224            5           2604    16530    projects project_id    DEFAULT     y   ALTER TABLE ONLY public.projects ALTER COLUMN project_id SET DEFAULT nextval('public.project_project_id_seq'::regclass);
 B   ALTER TABLE public.projects ALTER COLUMN project_id DROP DEFAULT;
       public          postgres    false    218    219    219            3           2604    16402    users user_id    DEFAULT     m   ALTER TABLE ONLY public.users ALTER COLUMN user_id SET DEFAULT nextval('public.user_user_id_seq'::regclass);
 <   ALTER TABLE public.users ALTER COLUMN user_id DROP DEFAULT;
       public          postgres    false    217    216    217            �          0    16665    emaillabels 
   TABLE DATA           9   COPY public.emaillabels (email_id, label_id) FROM stdin;
    public          postgres    false    225   �8       �          0    16574    emails 
   TABLE DATA           �   COPY public.emails (email_id, project_id, sender_address, receiver_address, subject, date_sent, email_content, file_path) FROM stdin;
    public          postgres    false    222   �8       �          0    16588    labels 
   TABLE DATA           B   COPY public.labels (label_id, label_name, created_by) FROM stdin;
    public          postgres    false    224   �8       �          0    16555    projectmembers 
   TABLE DATA           E   COPY public.projectmembers (project_id, user_id, status) FROM stdin;
    public          postgres    false    220   9       �          0    16527    projects 
   TABLE DATA           �   COPY public.projects (project_id, project_name, description, start_date, end_date, permission_tag, created_at, updated_at, owner_id) FROM stdin;
    public          postgres    false    219   $9       �          0    16399    users 
   TABLE DATA           U   COPY public.users (user_id, username, email, password, created_at, role) FROM stdin;
    public          postgres    false    217   A9       �           0    0    emails_email_id_seq    SEQUENCE SET     A   SELECT pg_catalog.setval('public.emails_email_id_seq', 1, true);
          public          postgres    false    221            �           0    0    emails_email_id_seq1    SEQUENCE SET     C   SELECT pg_catalog.setval('public.emails_email_id_seq1', 1, false);
          public          postgres    false    226            �           0    0    labels_label_id_seq    SEQUENCE SET     B   SELECT pg_catalog.setval('public.labels_label_id_seq', 1, false);
          public          postgres    false    223            �           0    0    project_project_id_seq    SEQUENCE SET     E   SELECT pg_catalog.setval('public.project_project_id_seq', 1, false);
          public          postgres    false    218            �           0    0    user_user_id_seq    SEQUENCE SET     ?   SELECT pg_catalog.setval('public.user_user_id_seq', 1, false);
          public          postgres    false    216            B           2606    16581    emails emails_pkey 
   CONSTRAINT     V   ALTER TABLE ONLY public.emails
    ADD CONSTRAINT emails_pkey PRIMARY KEY (email_id);
 <   ALTER TABLE ONLY public.emails DROP CONSTRAINT emails_pkey;
       public            postgres    false    222            D           2606    16593    labels labels_pkey 
   CONSTRAINT     V   ALTER TABLE ONLY public.labels
    ADD CONSTRAINT labels_pkey PRIMARY KEY (label_id);
 <   ALTER TABLE ONLY public.labels DROP CONSTRAINT labels_pkey;
       public            postgres    false    224            @           2606    16536    projects project_pkey 
   CONSTRAINT     [   ALTER TABLE ONLY public.projects
    ADD CONSTRAINT project_pkey PRIMARY KEY (project_id);
 ?   ALTER TABLE ONLY public.projects DROP CONSTRAINT project_pkey;
       public            postgres    false    219            :           2606    16409    users user_email_key 
   CONSTRAINT     P   ALTER TABLE ONLY public.users
    ADD CONSTRAINT user_email_key UNIQUE (email);
 >   ALTER TABLE ONLY public.users DROP CONSTRAINT user_email_key;
       public            postgres    false    217            <           2606    16405    users user_pkey 
   CONSTRAINT     R   ALTER TABLE ONLY public.users
    ADD CONSTRAINT user_pkey PRIMARY KEY (user_id);
 9   ALTER TABLE ONLY public.users DROP CONSTRAINT user_pkey;
       public            postgres    false    217            >           2606    16407    users user_username_key 
   CONSTRAINT     V   ALTER TABLE ONLY public.users
    ADD CONSTRAINT user_username_key UNIQUE (username);
 A   ALTER TABLE ONLY public.users DROP CONSTRAINT user_username_key;
       public            postgres    false    217            J           2606    16668 %   emaillabels emaillabels_email_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.emaillabels
    ADD CONSTRAINT emaillabels_email_id_fkey FOREIGN KEY (email_id) REFERENCES public.emails(email_id);
 O   ALTER TABLE ONLY public.emaillabels DROP CONSTRAINT emaillabels_email_id_fkey;
       public          postgres    false    225    222    4674            K           2606    16673 %   emaillabels emaillabels_label_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.emaillabels
    ADD CONSTRAINT emaillabels_label_id_fkey FOREIGN KEY (label_id) REFERENCES public.labels(label_id);
 O   ALTER TABLE ONLY public.emaillabels DROP CONSTRAINT emaillabels_label_id_fkey;
       public          postgres    false    225    224    4676            H           2606    16582    emails emails_project_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.emails
    ADD CONSTRAINT emails_project_id_fkey FOREIGN KEY (project_id) REFERENCES public.projects(project_id);
 G   ALTER TABLE ONLY public.emails DROP CONSTRAINT emails_project_id_fkey;
       public          postgres    false    222    4672    219            I           2606    16594    labels labels_created_by_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.labels
    ADD CONSTRAINT labels_created_by_fkey FOREIGN KEY (created_by) REFERENCES public.users(user_id);
 G   ALTER TABLE ONLY public.labels DROP CONSTRAINT labels_created_by_fkey;
       public          postgres    false    224    217    4668            F           2606    16558 .   projectmembers project_members_project_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.projectmembers
    ADD CONSTRAINT project_members_project_id_fkey FOREIGN KEY (project_id) REFERENCES public.projects(project_id);
 X   ALTER TABLE ONLY public.projectmembers DROP CONSTRAINT project_members_project_id_fkey;
       public          postgres    false    4672    220    219            G           2606    16563 +   projectmembers project_members_user_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.projectmembers
    ADD CONSTRAINT project_members_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(user_id);
 U   ALTER TABLE ONLY public.projectmembers DROP CONSTRAINT project_members_user_id_fkey;
       public          postgres    false    220    4668    217            E           2606    16568    projects project_owner_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.projects
    ADD CONSTRAINT project_owner_id_fkey FOREIGN KEY (owner_id) REFERENCES public.users(user_id);
 H   ALTER TABLE ONLY public.projects DROP CONSTRAINT project_owner_id_fkey;
       public          postgres    false    219    4668    217            �      x������ � �      �      x������ � �      �      x������ � �      �      x������ � �      �      x������ � �      �   f   x��1�0���Hd�71�G�.�)��v���^����}���l^+86�/�0��&�c&8�0����n�~�.>z'��6����|��]wVe*�����MZ     