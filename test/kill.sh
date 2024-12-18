#!/bin/bash

kill -9 $(lsof -t -i tcp:5177)