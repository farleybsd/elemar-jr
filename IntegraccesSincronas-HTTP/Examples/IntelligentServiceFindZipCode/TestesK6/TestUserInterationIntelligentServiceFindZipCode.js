import http from 'k6/http';
import { check } from 'k6';

/*
    Teste de carga usando taxa constante.

    Objetivo:
    Simular usuários chegando continuamente na API,
    independente do tempo de resposta.

    Cenário:
    - 50 novas requisições por segundo
    - durante 30 segundos
    - k6 pode criar até 50 usuários virtuais inicialmente
*/

export const options = {
    scenarios: {
        viaCep_load_test: {

            // Mantém taxa constante de chegada
            executor: 'constant-arrival-rate',

            // Duração total do teste
            duration: '30s',

            // Reserva inicial de VUs
            // Se faltar, o k6 cria mais dinamicamente
            preAllocatedVUs: 50,

            // Quantas iterações por unidade de tempo
            rate: 50,

            // Unidade usada pela taxa
            timeUnit: '1s',
        },
    },
};

export default function () {

    // CEP utilizado no teste
    const cep = '35057760';

    // Chamada para sua API
    const response = http.get(
        `http://localhost:5085/api/v1/SearchOne-ZipCode?cep=${cep}`
    );

    // Verifica se a API respondeu corretamente
    check(response, {
        'status é 200': (r) => r.status === 200,
    });
}

/*
Executar:

k6 run TestLoadIntelligentServiceFindZipCode.js
*/