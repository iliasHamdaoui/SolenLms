FROM nginx

EXPOSE 44395

COPY nginx/nginx.local.conf /etc/nginx/nginx.conf
COPY nginx/idp.crt /etc/ssl/certs/idp.local.solenlms.com.crt
COPY nginx/idp.key /etc/ssl/private/idp.local.solenlms.com.key
COPY nginx/api.crt /etc/ssl/certs/api.local.solenlms.com.crt
COPY nginx/api.key /etc/ssl/private/api.local.solenlms.com.key
COPY nginx/web.crt /etc/ssl/certs/web.local.solenlms.com.crt
COPY nginx/web.key /etc/ssl/private/web.local.solenlms.com.key