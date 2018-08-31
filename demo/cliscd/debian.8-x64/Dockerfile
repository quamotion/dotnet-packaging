FROM debian:8

COPY cliscd.1.0.0.debian.8-x64.deb /pkg/
COPY reference.txt /pkg/

RUN apt-get update \
&& dpkg -i /pkg/cliscd.1.0.0.debian.8-x64.deb || apt-get install -f -y

RUN ls -a /usr/share/cliscd >> ~/testoutput.log 2>&1 || exit 0
RUN ls -a /etc/cliscd >> ~/testoutput.log 2>&1 || exit 0
RUN ls -a ~/.cliscd >> ~/testoutput.log 2>&1 || exit 0

RUN diff -w /pkg/reference.txt ~/testoutput.log

CMD [ "/usr/share/cliscd/cliscd" ]
