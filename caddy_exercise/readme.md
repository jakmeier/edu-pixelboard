# Basic auth

```bash
# generate hash
docker run -it --rm caddy:2 caddy hash-password
```

```conf
# Caddyfile
:80 {
    basic_auth {
        my_username INSERT_HASH
    }
    root * /srv
    file_server
}
```

# Reverse Proxy

```conf
# Caddyfile
:80 {

    # Optional
    basic_auth {
        my_username INSERT_HASH
    }

    handle_path /paduk* {
        reverse_proxy host.docker.internal:5085
    }

    handle {
        root * /srv
        file_server
    }
}
```
