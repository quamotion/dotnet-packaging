FROM opensuse/leap:42.3

ENV rid=opensuse.42.3-x64
COPY cliscd.1.0.0.$rid.rpm /pkg/
COPY reference.txt /pkg/

RUN zypper --non-interactive --no-gpg-checks install /pkg/cliscd.1.0.0.$rid.rpm

RUN ls -a /usr/share/cliscd >> ~/testoutput.log 2>&1 || exit 0
RUN ls -a /etc/cliscd >> ~/testoutput.log 2>&1 || exit 0
RUN ls -a ~/.cliscd >> ~/testoutput.log 2>&1 || exit 0

RUN diff -w /pkg/reference.txt ~/testoutput.log

CMD [ "/usr/share/cliscd/cliscd" ]
