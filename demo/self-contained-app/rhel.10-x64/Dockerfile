FROM redhat/ubi10

ENV rid=linux-x64
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

COPY self-contained-app.1.0.0.$rid.rpm /pkg/
COPY reference.txt /pkg/

RUN yum update -y \
&& yum install -y diffutils \
&& yum install -y /pkg/self-contained-app.1.0.0.$rid.rpm

RUN ls -a /usr/share/self-contained-app >> ~/testoutput.log 2>&1 || exit 0
RUN ls -a /etc/self-contained-app >> ~/testoutput.log 2>&1 || exit 0
RUN ls -a ~/.self-contained-app >> ~/testoutput.log 2>&1 || exit 0

RUN diff -w /pkg/reference.txt ~/testoutput.log

CMD [ "/usr/share/self-contained-app/self-contained-app" ]
