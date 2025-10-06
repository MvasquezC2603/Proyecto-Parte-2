import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Marcador } from './marcador';

describe('Marcador', () => {
  let component: Marcador;
  let fixture: ComponentFixture<Marcador>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Marcador]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Marcador);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
