import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { EmployeeService } from './employee.service';
import { Employee } from './employee.model';
import { DatePipe } from '@angular/common';

describe('EmployeeService', () => {
  let service: EmployeeService;
  let httpMock: HttpTestingController;
  let datePipe: DatePipe;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        EmployeeService,
        DatePipe
      ]
    });

    service = TestBed.inject(EmployeeService);
    httpMock = TestBed.inject(HttpTestingController);
    datePipe = TestBed.inject(DatePipe);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should retrieve all employees', () => {

  // Fecha fija, sin conversiones ni ISO ni nada
  const fixedDate = '2025-01-01T12:00:00.000Z';

  // Dummy data que simula la respuesta del backend
  const dummyEmployees: Employee[] = [
    new Employee(1, 'John Doe', fixedDate),
    new Employee(2, 'Jane Smith', fixedDate)
  ];

  service.getAllEmployee().subscribe(employees => {
    expect(employees.length).toBe(2);

    employees.forEach((employee, index) => {
      expect(employee.createdDate).toEqual(dummyEmployees[index].createdDate);
    });
  });

  const req = httpMock.expectOne(`${service.apiUrlEmployee}/getall`);
  expect(req.request.method).toBe('GET');
  req.flush(dummyEmployees);
});
});
