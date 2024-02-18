{{- define "abpzerotemplate.global.env" -}}
- name: "App__ServerRootAddress"
  value: "{{ .Values.global.wwwHostUrl }}"
- name: "App__ClientRootAddress"
  value: "{{ .Values.global.wwwAngularUrl }}"  
- name: "App__CorsOrigins"
  value: "{{ .Values.global.wwwAngularUrl }}"    
{{- end }}