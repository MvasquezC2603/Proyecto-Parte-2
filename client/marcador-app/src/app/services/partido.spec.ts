import { TestBed } from '@angular/core/testing';

import { Partido } from './partido';

describe('Partido', () => {
  let service: Partido;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Partido);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
