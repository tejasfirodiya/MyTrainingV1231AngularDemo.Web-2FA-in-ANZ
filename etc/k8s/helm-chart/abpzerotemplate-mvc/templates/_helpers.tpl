{{- define "abpzerotemplate.global.env" -}}
- name: "App__WebSiteRootAddress"
  value: "{{ .Values.global.wwwUrl }}"
{{- end }}