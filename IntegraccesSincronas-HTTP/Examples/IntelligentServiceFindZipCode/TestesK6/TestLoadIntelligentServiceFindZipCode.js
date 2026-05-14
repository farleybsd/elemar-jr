import http from 'k6/http';
import { check } from 'k6';

/*
    Configuração do teste de carga.

    vus = Virtual Users (usuários virtuais)

    O k6 criará até 50 usuários simultâneos executando
    a função "default" continuamente.

    duration = tempo total do teste

    Durante 30 segundos os usuários ficarão enviando
    requisições sem parar.

    Cenário simulado:

    50 usuários acessando a API ao mesmo tempo
    durante 30 segundos.
*/
export const options = {
  vus: 50,
  duration: '30s',
};

export default function () {

  const cep = '35057760';

  const res = http.get(
    `http://localhost:5085/api/v1/SearchOne-ZipCode?cep=${cep}`
  );

  
  check(res, {
    'status 200': (r) => r.status === 200,
  });
}


/*
 * k6 run script.js
*/