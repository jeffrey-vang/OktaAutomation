exit_after_auth = false
pid_file = "/home/vault/.pid"

auto_auth {
  method "kubernetes" {
    config {
      role = "{{ .Values.vault.role }}"
    }
    mount_path = "auth/{{ .Values.vault.mountPath }}"
  }
  sink "file" {
    config {
      path = "/home/vault/.token"
    }
  }
}

template {
  destination = "/vault/secrets/secrets.json"
  error_on_missing_key = true
  left_delimiter = "{%"
  right_delimiter = "%}"
  source = "/vault/configs/secrets.ctmpl"
}

vault {
  address = "{{ .Values.vault.address }}"
  ca_cert = "/vault/tls/tls.crt"
}