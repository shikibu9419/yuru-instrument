# -*- coding:utf-8 -*-
import socket

host = '127.0.0.1'
port = 8888

serversock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
serversock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
serversock.bind((host, port))
serversock.listen(10)

print('Waiting for connections...')
clientsock, client_address = serversock.accept()

while True:
    rcvmsg = clientsock.recv(1024)
    print('Received -> %s' % (rcvmsg))

    if rcvmsg == '':
      break

    clientsock.sendall(f'received: {rcvmsg}'.encode())

clientsock.close()