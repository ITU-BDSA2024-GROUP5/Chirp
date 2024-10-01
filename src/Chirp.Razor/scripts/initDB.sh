#!/usr/bin/env bash
sqlite3 /Temp/chirp.db < ../data/schema.sql
sqlite3 /Temp/chirp.db < ../data/dump.sql
