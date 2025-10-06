import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Faltas } from './faltas';

describe('Faltas', () => {
  let component: Faltas;
  let fixture: ComponentFixture<Faltas>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Faltas]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Faltas);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
