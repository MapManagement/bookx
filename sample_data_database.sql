--- Languages ---
insert into public."Languages" values (1, 'German');
insert into public."Languages" values (2, 'English');

--- Genres ---
insert into public."Genres" values (1, 'Thriller');
insert into public."Genres" values (2, 'Novel');
insert into public."Genres" values (3, 'Science-Fiction');
insert into public."Genres" values (4, 'Western');

--- Publishers ---
insert into public."Publishers" values (1, 'Penguin Books Ltd');
insert into public."Publishers" values (2, 'Knaur');
insert into public."Publishers" values (3, 'Diogenes Verlag AG');
insert into public."Publishers" values (4, 'Pan Macmillan');

--- Authors ---
insert into public."Authors" values (1, 'Cormac', 'McCarthy', '1933-07-20 00:00:00-00');
insert into public."Authors" values (2, 'Sebastian', 'Fitzek', '1971-10-13 00:00:00-00');
insert into public."Authors" values (3, 'George', 'Orwell', '1903-06-25 00:00:00-00');
insert into public."Authors" values (4, 'Patrick', 'Süskind', '1949-03-26 00:00:00-00');

--- Books ---
insert into public."Books" values
    ('9781447289463', 'Blood Meridian', 'my-path', 368, 'my-link', 'my-blurb', '2015-08-13 00:00:00-00', 2, 4);
insert into public."Books" values
    ('9783426554791', 'Der Seelenbrecher', 'my-path', 368, 'my-link', 'my-blurb', '2010-04-22 00:00:00-00', 1, 2);
insert into public."Books" values
    ('9783257601756', 'Das Parfum', 'my-path', 336, 'my-link', 'my-blurb', '2012-09-25 00:00:00-00', 1, 3);
insert into public."Books" values
    ('9780141036144', '1984', 'my-path', 336, 'my-link', 'my-blurb', '2008-07-23 00:00:00-00', 2, 1);

--- AuthorBook ---
insert into public."AuthorBook" values(1, '9781447289463');
insert into public."AuthorBook" values(2, '9783426554791');
insert into public."AuthorBook" values(3, '9780141036144');
insert into public."AuthorBook" values(4, '9783257601756');

--- BookGenre ---
insert into public."BookGenre" values('9781447289463', 4);
insert into public."BookGenre" values('9783426554791', 1);
insert into public."BookGenre" values('9783257601756', 2);
insert into public."BookGenre" values('9780141036144', 3);
